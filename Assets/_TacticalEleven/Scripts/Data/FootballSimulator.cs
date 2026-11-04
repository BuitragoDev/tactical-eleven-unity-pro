using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// F8 — Motor de partido de fútbol. Simula 90 minutos a partir de los onces y
/// devuelve marcador, eventos y estadísticas individuales.
///
/// Es determinista con un <see cref="IRandomSource"/> sembrado: el mismo partido
/// con la misma semilla produce el mismo resultado.
/// </summary>
public static class FootballSimulator
{
    public const int RegulationMinutes = 90;

    public static MatchSimulationResult Simulate(MatchTeamSetup home, MatchTeamSetup away,
                                                 IRandomSource random = null)
    {
        if (home == null) throw new ArgumentNullException(nameof(home));
        if (away == null) throw new ArgumentNullException(nameof(away));

        random ??= new SeededRandomSource(1);

        var result = new MatchSimulationResult
        {
            HomeTeamId = home.Team?.Id ?? 0,
            AwayTeamId = away.Team?.Id ?? 0
        };

        var homeStats = BuildPlayerStats(home);
        var awayStats = BuildPlayerStats(away);

        var homeStrength = Evaluate(home);
        var awayStrength = Evaluate(away);

        // ── Posesión ───────────────────────────────────────────────────────
        int possession = RoundToInt(50f + (homeStrength.Midfield - awayStrength.Midfield) * 0.8f);
        possession += random.Range(-2, 3);
        possession = ClampInt(possession, 28, 72);
        result.HomePossession = possession;
        result.AwayPossession = 100 - possession;

        // ── Goles esperados ────────────────────────────────────────────────
        float homeXg = Clamp(1.30f * (homeStrength.Attack / Max(45f, awayStrength.Defense)) * 1.12f, 0.15f, 4.5f);
        float awayXg = Clamp(1.10f * (awayStrength.Attack / Max(45f, homeStrength.Defense)), 0.10f, 4.0f);

        var homeGoalMinutes = SampleGoalMinutes(homeXg, random);
        var awayGoalMinutes = SampleGoalMinutes(awayXg, random);
        result.HomeScore = homeGoalMinutes.Count;
        result.AwayScore = awayGoalMinutes.Count;

        // ── Tiros, paradas, córners y faltas ───────────────────────────────
        result.HomeShots = Math.Max(result.HomeScore,
            RoundToInt(homeXg / 0.115f) + random.Range(0, 4));
        result.AwayShots = Math.Max(result.AwayScore,
            RoundToInt(awayXg / 0.115f) + random.Range(0, 4));

        result.HomeShotsOnTarget = ClampInt(
            result.HomeScore + random.Range(0, Math.Max(1, result.HomeShots / 3)),
            result.HomeScore, result.HomeShots);
        result.AwayShotsOnTarget = ClampInt(
            result.AwayScore + random.Range(0, Math.Max(1, result.AwayShots / 3)),
            result.AwayScore, result.AwayShots);

        result.HomeCorners = RoundToInt(result.HomeShots * 0.42f) + random.Range(0, 3);
        result.AwayCorners = RoundToInt(result.AwayShots * 0.42f) + random.Range(0, 3);

        result.HomeFouls = 8 + random.Range(0, 7);
        result.AwayFouls = 8 + random.Range(0, 7);

        result.HomeYellow = random.Range(1, 4);
        result.AwayYellow = random.Range(1, 4);
        result.HomeRed = random.Value < 0.06f ? 1 : 0;
        result.AwayRed = random.Value < 0.06f ? 1 : 0;

        // ── Goles: goleador y asistencia ───────────────────────────────────
        AddGoalEvents(result, home, homeStats, homeGoalMinutes, random);
        AddGoalEvents(result, away, awayStats, awayGoalMinutes, random);

        // ── Reparto de tiros ───────────────────────────────────────────────
        DistributeShots(home, homeStats, result.HomeShots - result.HomeScore,
            Math.Max(0, result.HomeShotsOnTarget - result.HomeScore), random);
        DistributeShots(away, awayStats, result.AwayShots - result.AwayScore,
            Math.Max(0, result.AwayShotsOnTarget - result.AwayScore), random);

        // ── Porteros ───────────────────────────────────────────────────────
        ApplyGoalkeeperStats(home, homeStats, result.AwayScore,
            Math.Max(0, result.AwayShotsOnTarget - result.AwayScore));
        ApplyGoalkeeperStats(away, awayStats, result.HomeScore,
            Math.Max(0, result.HomeShotsOnTarget - result.HomeScore));

        // ── Tarjetas, faltas, cambios y pases ──────────────────────────────
        AddCards(result, home, homeStats, result.HomeYellow, result.HomeRed, random);
        AddCards(result, away, awayStats, result.AwayYellow, result.AwayRed, random);

        DistributeFouls(home, homeStats, result.HomeFouls, random);
        DistributeFouls(away, awayStats, result.AwayFouls, random);

        AddSubstitutions(result, home, homeStats, random);
        AddSubstitutions(result, away, awayStats, random);

        ApplyPassing(home, homeStats, result.HomePossession, random);
        ApplyPassing(away, awayStats, result.AwayPossession, random);

        // ── Cierre y notas ─────────────────────────────────────────────────
        int added = 1 + random.Range(0, 5);
        result.Events.Add(new SimulatedEvent { Minute = 0, Type = MatchEventType.KickOff, TeamId = result.HomeTeamId });
        result.Events.Add(new SimulatedEvent
        {
            Minute = 90,
            Type = MatchEventType.AddedTime,
            TeamId = result.HomeTeamId,
            Detail = added.ToString()
        });
        result.Events.Add(new SimulatedEvent
        {
            Minute = 90 + added,
            Type = MatchEventType.FullTime,
            TeamId = result.HomeTeamId
        });

        ApplyRatings(result, home, homeStats, away, awayStats, random);

        result.Players.AddRange(homeStats);
        result.Players.AddRange(awayStats);

        result.Events.Sort((a, b) => a.Minute != b.Minute
            ? a.Minute.CompareTo(b.Minute)
            : a.Type.CompareTo(b.Type));

        return result;
    }

    // ── FUERZA DEL EQUIPO ─────────────────────────────────────────────────

    private struct Strength
    {
        public float Attack;
        public float Midfield;
        public float Defense;
    }

    private static Strength Evaluate(MatchTeamSetup setup)
    {
        var slots = FormationRules.Slots(setup.Formation);

        float attack = 0f, midfield = 0f, defense = 0f;
        int attackCount = 0, midfieldCount = 0, defenseCount = 0;

        int count = Math.Min(setup.Starters.Count, slots.Length);
        for (int i = 0; i < count; i++)
        {
            var player = setup.Starters[i];
            if (player == null) continue;

            float rating = player.Overall *
                           FormationRules.Compatibility((PlayerPosition)player.MainPosition, slots[i]);

            int line = PlayerPositionNames.LineGroup((int)slots[i]);
            if (line == 0 || line == 1)
            {
                defense += rating;
                defenseCount++;
            }
            else if (line == 2)
            {
                midfield += rating;
                midfieldCount++;
            }
            else
            {
                attack += rating;
                attackCount++;
            }
        }

        return new Strength
        {
            Attack = attackCount > 0 ? attack / attackCount : 60f,
            Midfield = midfieldCount > 0 ? midfield / midfieldCount : 60f,
            Defense = defenseCount > 0 ? defense / defenseCount : 60f
        };
    }

    // ── GOLES ─────────────────────────────────────────────────────────────

    private static List<int> SampleGoalMinutes(float xg, IRandomSource random)
    {
        var minutes = new List<int>();
        float perMinute = Clamp(xg / RegulationMinutes, 0f, 1f);

        for (int minute = 1; minute <= RegulationMinutes; minute++)
            if (random.Value < perMinute) minutes.Add(minute);

        return minutes;
    }

    private static void AddGoalEvents(MatchSimulationResult result, MatchTeamSetup setup,
                                      List<SimulatedPlayerStats> stats, List<int> minutes,
                                      IRandomSource random)
    {
        var slots = FormationRules.Slots(setup.Formation);

        foreach (int minute in minutes)
        {
            var scorer = PickWeighted(setup.Starters, slots, random, GoalWeight);
            if (scorer == null) continue;

            var scorerStats = stats.FirstOrDefault(s => s.PlayerId == scorer.Id);
            if (scorerStats == null) continue;

            scorerStats.Goals++;
            scorerStats.Shots = Math.Max(scorerStats.Shots + 1, scorerStats.Goals);
            scorerStats.ShotsOnTarget++;

            int assistId = 0;
            if (random.Value < 0.72f)
            {
                var assister = PickWeighted(
                    setup.Starters.Where(p => p.Id != scorer.Id).ToList(), slots, random, AssistWeight);
                if (assister != null)
                {
                    assistId = assister.Id;
                    var assistStats = stats.FirstOrDefault(s => s.PlayerId == assister.Id);
                    if (assistStats != null) assistStats.Assists++;
                }
            }

            result.Events.Add(new SimulatedEvent
            {
                Minute = minute,
                Type = MatchEventType.Goal,
                TeamId = setup.Team?.Id ?? 0,
                PlayerId = scorer.Id,
                SecondaryPlayerId = assistId
            });
        }
    }

    private static float GoalWeight(PlayerDataDB player, PlayerPosition slot)
    {
        float baseWeight = slot switch
        {
            PlayerPosition.ST or PlayerPosition.CF => 4.0f,
            PlayerPosition.LW or PlayerPosition.RW => 2.2f,
            PlayerPosition.CAM => 1.8f,
            PlayerPosition.LM or PlayerPosition.RM => 1.1f,
            PlayerPosition.CM => 0.9f,
            PlayerPosition.CDM => 0.5f,
            PlayerPosition.GK => 0f,
            _ => 0.35f
        };

        float finishing = (player.Finishing + player.OffTheBall) / 2f / 70f;
        return baseWeight * Clamp(finishing, 0.4f, 1.8f) * (player.Overall / 70f);
    }

    private static float AssistWeight(PlayerDataDB player, PlayerPosition slot)
    {
        float baseWeight = slot switch
        {
            PlayerPosition.CAM => 2.2f,
            PlayerPosition.CM => 1.6f,
            PlayerPosition.LW or PlayerPosition.RW => 1.8f,
            PlayerPosition.LM or PlayerPosition.RM => 1.4f,
            PlayerPosition.ST or PlayerPosition.CF => 1.0f,
            PlayerPosition.CDM => 0.8f,
            PlayerPosition.GK => 0f,
            _ => 0.4f
        };

        float vision = (player.Passing + player.Vision + player.Crossing) / 3f / 70f;
        return baseWeight * Clamp(vision, 0.4f, 1.8f) * (player.Overall / 70f);
    }

    // ── TIROS / PARADAS / FALTAS / PASES ──────────────────────────────────

    private static void DistributeShots(MatchTeamSetup setup, List<SimulatedPlayerStats> stats,
                                        int shots, int onTarget, IRandomSource random)
    {
        var slots = FormationRules.Slots(setup.Formation);

        for (int i = 0; i < shots; i++)
        {
            var shooter = PickWeighted(setup.Starters, slots, random, GoalWeight);
            var stat = shooter != null ? stats.FirstOrDefault(s => s.PlayerId == shooter.Id) : null;
            if (stat != null) stat.Shots++;
        }

        for (int i = 0; i < onTarget; i++)
        {
            var shooter = PickWeighted(setup.Starters, slots, random, GoalWeight);
            var stat = shooter != null ? stats.FirstOrDefault(s => s.PlayerId == shooter.Id) : null;
            if (stat != null && stat.ShotsOnTarget < stat.Shots) stat.ShotsOnTarget++;
        }
    }

    private static void ApplyGoalkeeperStats(MatchTeamSetup setup, List<SimulatedPlayerStats> stats,
                                             int goalsConceded, int saves)
    {
        var keeper = setup.Starters.FirstOrDefault(p => p.MainPosition == (int)PlayerPosition.GK);
        if (keeper == null) return;

        var stat = stats.FirstOrDefault(s => s.PlayerId == keeper.Id);
        if (stat == null) return;

        stat.GoalsConceded = goalsConceded;
        stat.Saves = Math.Max(0, saves);
    }

    private static void DistributeFouls(MatchTeamSetup setup, List<SimulatedPlayerStats> stats,
                                        int fouls, IRandomSource random)
    {
        if (setup.Starters.Count == 0) return;

        for (int i = 0; i < fouls; i++)
        {
            var player = setup.Starters[random.Range(0, setup.Starters.Count)];
            var stat = stats.FirstOrDefault(s => s.PlayerId == player.Id);
            if (stat != null) stat.Fouls++;
        }
    }

    private static void ApplyPassing(MatchTeamSetup setup, List<SimulatedPlayerStats> stats,
                                     int possession, IRandomSource random)
    {
        int teamPasses = 250 + possession * 4 + random.Range(-40, 40);
        int outfield = Math.Max(1, setup.Starters.Count(p => p.MainPosition != (int)PlayerPosition.GK));

        foreach (var stat in stats)
        {
            var player = setup.Starters.FirstOrDefault(p => p.Id == stat.PlayerId);
            if (player == null || player.MainPosition == (int)PlayerPosition.GK) continue;

            stat.Passes = Math.Max(0, teamPasses / outfield + random.Range(-8, 9));
            stat.PassAccuracy = random.Range(68, 89);
            stat.Tackles = random.Range(0, 4);
            stat.Interceptions = random.Range(0, 4);
        }
    }

    // ── TARJETAS ──────────────────────────────────────────────────────────

    private static void AddCards(MatchSimulationResult result, MatchTeamSetup setup,
                                 List<SimulatedPlayerStats> stats, int yellows, int reds,
                                 IRandomSource random)
    {
        var candidates = setup.Starters.Where(p => p.MainPosition != (int)PlayerPosition.GK).ToList();
        if (candidates.Count == 0) return;

        for (int i = 0; i < yellows; i++)
        {
            var player = PickWeighted(candidates, null, random, CardWeight);
            if (player == null) continue;

            var stat = stats.FirstOrDefault(s => s.PlayerId == player.Id);
            if (stat == null) continue;

            stat.YellowCard++;
            result.Events.Add(new SimulatedEvent
            {
                Minute = random.Range(12, 89),
                Type = MatchEventType.YellowCard,
                TeamId = setup.Team?.Id ?? 0,
                PlayerId = player.Id
            });
        }

        for (int i = 0; i < reds; i++)
        {
            var player = PickWeighted(candidates, null, random, CardWeight);
            if (player == null) continue;

            var stat = stats.FirstOrDefault(s => s.PlayerId == player.Id);
            if (stat == null) continue;

            stat.RedCard = 1;
            result.Events.Add(new SimulatedEvent
            {
                Minute = random.Range(20, 89),
                Type = MatchEventType.RedCard,
                TeamId = setup.Team?.Id ?? 0,
                PlayerId = player.Id
            });
        }
    }

    private static float CardWeight(PlayerDataDB player, PlayerPosition slot)
        => 1f + (player.Aggression + player.WorkRate) / 40f;

    // ── CAMBIOS ───────────────────────────────────────────────────────────

    private static void AddSubstitutions(MatchSimulationResult result, MatchTeamSetup setup,
                                        List<SimulatedPlayerStats> stats, IRandomSource random)
    {
        int substitutions = Math.Min(3, setup.Bench.Count);
        if (substitutions == 0) return;

        var minutes = new List<int>();
        for (int i = 0; i < substitutions; i++)
            minutes.Add(random.Range(55, 81));
        minutes.Sort();

        var available = setup.Starters
            .Where(p => p.MainPosition != (int)PlayerPosition.GK)
            .ToList();

        int benchIndex = 0;
        foreach (int minute in minutes)
        {
            if (available.Count == 0 || benchIndex >= setup.Bench.Count) break;

            int index = random.Range(0, available.Count);
            var leaving = available[index];
            available.RemoveAt(index);

            var entering = setup.Bench[benchIndex++];

            var leavingStats = stats.FirstOrDefault(s => s.PlayerId == leaving.Id);
            if (leavingStats != null)
                leavingStats.Minutes = Math.Min(leavingStats.Minutes, minute);

            stats.Add(new SimulatedPlayerStats
            {
                PlayerId = entering.Id,
                TeamId = setup.Team?.Id ?? 0,
                Minutes = Math.Max(0, RegulationMinutes - minute),
                Started = false
            });

            result.Events.Add(new SimulatedEvent
            {
                Minute = minute,
                Type = MatchEventType.SubstitutionOn,
                TeamId = setup.Team?.Id ?? 0,
                PlayerId = entering.Id,
                SecondaryPlayerId = leaving.Id
            });
        }
    }

    // ── NOTAS Y MVP ───────────────────────────────────────────────────────

    private static void ApplyRatings(MatchSimulationResult result,
                                     MatchTeamSetup home, List<SimulatedPlayerStats> homeStats,
                                     MatchTeamSetup away, List<SimulatedPlayerStats> awayStats,
                                     IRandomSource random)
    {
        ApplyTeamRatings(homeStats, home, result.HomeScore, result.AwayScore, random);
        ApplyTeamRatings(awayStats, away, result.AwayScore, result.HomeScore, random);

        double best = -1;
        int mvp = 0;

        foreach (var stat in homeStats.Concat(awayStats))
        {
            if (stat.Minutes <= 0) continue;
            if (stat.Rating > best)
            {
                best = stat.Rating;
                mvp = stat.PlayerId;
            }
        }

        result.MvpPlayerId = mvp;
    }

    private static void ApplyTeamRatings(List<SimulatedPlayerStats> stats, MatchTeamSetup setup,
                                         int goalsFor, int goalsAgainst, IRandomSource random)
    {
        float teamBonus = goalsFor > goalsAgainst ? 0.35f
            : goalsFor == goalsAgainst ? 0.05f
            : -0.25f;

        foreach (var stat in stats)
        {
            if (stat.Minutes <= 0) continue;

            var player = setup.Starters.FirstOrDefault(p => p.Id == stat.PlayerId)
                         ?? setup.Bench.FirstOrDefault(p => p.Id == stat.PlayerId);

            float rating = 6.2f;
            rating += stat.Goals * 1.0f;
            rating += stat.Assists * 0.55f;
            rating -= stat.YellowCard * 0.15f;
            rating -= stat.RedCard * 0.8f;

            if (player != null && player.MainPosition == (int)PlayerPosition.GK)
                rating += stat.Saves * 0.12f - stat.GoalsConceded * 0.2f;

            rating += teamBonus;
            rating += random.Range(-0.3f, 0.3f);

            // Un suplente con pocos minutos no puede ser el mejor del partido.
            if (stat.Minutes < 30)
                rating = 6.0f + (rating - 6.0f) * (stat.Minutes / 30f);

            stat.Rating = Math.Round(Clamp(rating, 3.0f, 10.0f), 2);
        }
    }

    // ── UTILIDADES ────────────────────────────────────────────────────────

    private static List<SimulatedPlayerStats> BuildPlayerStats(MatchTeamSetup setup)
        => setup.Starters
            .Where(p => p != null)
            .Select(p => new SimulatedPlayerStats
            {
                PlayerId = p.Id,
                TeamId = setup.Team?.Id ?? 0,
                Minutes = RegulationMinutes,
                Started = true
            })
            .ToList();

    private static PlayerDataDB PickWeighted(List<PlayerDataDB> players, PlayerPosition[] slots,
                                             IRandomSource random,
                                             Func<PlayerDataDB, PlayerPosition, float> weight)
    {
        if (players == null || players.Count == 0) return null;

        var weights = new float[players.Count];
        float total = 0f;

        for (int i = 0; i < players.Count; i++)
        {
            var slot = slots != null && i < slots.Length
                ? slots[i]
                : (PlayerPosition)players[i].MainPosition;
            weights[i] = Math.Max(0.01f, weight(players[i], slot));
            total += weights[i];
        }

        if (total <= 0f) return players[random.Range(0, players.Count)];

        float pick = random.Value * total;
        for (int i = 0; i < players.Count; i++)
        {
            pick -= weights[i];
            if (pick <= 0f) return players[i];
        }

        return players[players.Count - 1];
    }

    private static int RoundToInt(float value) => (int)Math.Round(value);

    private static float Clamp(float value, float min, float max)
        => value < min ? min : value > max ? max : value;

    private static int ClampInt(int value, int min, int max)
        => value < min ? min : value > max ? max : value;

    private static float Max(float a, float b) => a > b ? a : b;
}
