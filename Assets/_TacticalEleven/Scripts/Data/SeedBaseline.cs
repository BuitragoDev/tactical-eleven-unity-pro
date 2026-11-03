using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Huella del seed estático. Se calcula a partir de las mismas listas que se siembran
/// (competiciones, equipos, palmarés y premios), así que **cambia sola** cuando cambia
/// el seed y sirve para reconstruir `template.db` sin tocar constantes a mano.
///
/// Es frescura, no validez: la validez la decide <see cref="SeedAudit"/> y los tests.
/// </summary>
public static class SeedBaseline
{
    /// <summary>Versión de esquema que espera el seed (ver `DatabaseManager.SchemaVersion`).</summary>
    public const int SchemaVersion = 1;

    /// <summary>
    /// Versión manual del seed de jugadores. Se sube al tocar `PlayerSeeder`
    /// (4.000+ filas: no se hashean para no penalizar la apertura de partida).
    /// </summary>
    public const int PlayerSeedVersion = 2;

    /// <summary>Huella actual del seed (determinista entre ejecuciones).</summary>
    public static string Fingerprint
    {
        get
        {
            var sb = new StringBuilder();

            sb.Append("players-v").Append(PlayerSeedVersion).Append(';');

            foreach (var l in CompetitionSeeder.Build().OrderBy(l => l.Id))
                sb.Append(l.Id).Append('|').Append(l.Name).Append('|').Append(l.Type)
                  .Append('|').Append(l.TeamCount).Append(';');

            foreach (var t in TeamSeeder.Build().OrderBy(t => t.Id))
                sb.Append(t.Id).Append('|').Append(t.Name).Append('|').Append(t.ShortName)
                  .Append('|').Append(t.LeagueId).Append('|').Append(t.StadiumCapacity)
                  .Append('|').Append(t.Overall).Append(';');

            foreach (var f in PalmaresSeeder.FinalsData)
                sb.Append(f.Season).Append('|').Append(f.CompetitionId).Append('|')
                  .Append(f.ChampionTeamId).Append('|').Append(f.FinalistTeamId).Append(';');

            foreach (var a in PalmaresSeeder.AwardsData)
                sb.Append(a.Season).Append('|').Append(a.Winner).Append('|').Append(a.TeamId).Append(';');

            foreach (var b in GoldenBootSeeder.Data)
                sb.Append(b.Season).Append('|').Append(b.Winner).Append('|').Append(b.TeamId)
                  .Append('|').Append(b.Goals).Append(';');

            return Hash(sb.ToString());
        }
    }

    /// <summary>Huella de un subconjunto (útil en tests o herramientas del editor).</summary>
    public static string Compute(IEnumerable<LeagueDataDB> leagues, IEnumerable<TeamDataDB> teams)
    {
        var sb = new StringBuilder();
        foreach (var l in (leagues ?? Enumerable.Empty<LeagueDataDB>()).OrderBy(l => l.Id))
            sb.Append(l.Id).Append('|').Append(l.Name).Append('|').Append(l.Type)
              .Append('|').Append(l.TeamCount).Append(';');
        foreach (var t in (teams ?? Enumerable.Empty<TeamDataDB>()).OrderBy(t => t.Id))
            sb.Append(t.Id).Append('|').Append(t.Name).Append('|').Append(t.ShortName)
              .Append('|').Append(t.LeagueId).Append('|').Append(t.StadiumCapacity)
              .Append('|').Append(t.Overall).Append(';');
        return Hash(sb.ToString());
    }

    private static string Hash(string content)
    {
        using var md5 = MD5.Create();
        byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(content));
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}
