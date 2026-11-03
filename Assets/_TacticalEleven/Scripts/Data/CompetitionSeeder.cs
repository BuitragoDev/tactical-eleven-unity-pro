using System.Collections.Generic;

/// <summary>
/// Competiciones del juego. Los ids coinciden con `PalmaresSeeder.CompetitionId`
/// (1-8) para no remapear el histórico; la Supercopa entra como 9.
/// </summary>
public static class CompetitionSeeder
{
    public const string TypeLeague = "league";
    public const string TypeCup = "cup";
    public const string TypeSuperCup = "supercup";
    public const string TypeEurope = "europe";

    public static List<LeagueDataDB> Build()
    {
        return new List<LeagueDataDB>
        {
            new LeagueDataDB
            {
                Id = 1, Name = "LaLiga EA Sports", ShortName = "LALIGA", Country = "Spain",
                Division = 1, Tier = 1, GroupIndex = 0, Type = TypeLeague,
                TeamCount = 20, Promotions = 0, Relegations = 3, EuropeSlots = 7, Logo = "laliga"
            },
            new LeagueDataDB
            {
                Id = 2, Name = "LaLiga Hypermotion", ShortName = "LALIGA2", Country = "Spain",
                Division = 2, Tier = 2, GroupIndex = 0, Type = TypeLeague,
                TeamCount = 22, Promotions = 3, Relegations = 4, EuropeSlots = 0, Logo = "laliga2"
            },
            new LeagueDataDB
            {
                Id = 3, Name = "Primera Federación — Grupo I", ShortName = "1RFEF-G1", Country = "Spain",
                Division = 3, Tier = 3, GroupIndex = 1, Type = TypeLeague,
                TeamCount = 20, Promotions = 2, Relegations = 5, EuropeSlots = 0, Logo = "primera_federacion"
            },
            new LeagueDataDB
            {
                Id = 4, Name = "Primera Federación — Grupo II", ShortName = "1RFEF-G2", Country = "Spain",
                Division = 3, Tier = 3, GroupIndex = 2, Type = TypeLeague,
                TeamCount = 20, Promotions = 2, Relegations = 5, EuropeSlots = 0, Logo = "primera_federacion"
            },
            new LeagueDataDB
            {
                Id = 5, Name = "UEFA Champions League", ShortName = "UCL", Country = "Europe",
                Division = 0, Tier = 0, GroupIndex = 0, Type = TypeEurope,
                TeamCount = 36, Promotions = 0, Relegations = 0, EuropeSlots = 0, Logo = "champions"
            },
            new LeagueDataDB
            {
                Id = 6, Name = "UEFA Europa League", ShortName = "UEL", Country = "Europe",
                Division = 0, Tier = 0, GroupIndex = 0, Type = TypeEurope,
                TeamCount = 36, Promotions = 0, Relegations = 0, EuropeSlots = 0, Logo = "europa_league"
            },
            new LeagueDataDB
            {
                Id = 7, Name = "UEFA Conference League", ShortName = "UECL", Country = "Europe",
                Division = 0, Tier = 0, GroupIndex = 0, Type = TypeEurope,
                TeamCount = 36, Promotions = 0, Relegations = 0, EuropeSlots = 0, Logo = "conference"
            },
            new LeagueDataDB
            {
                Id = 8, Name = "Copa del Rey", ShortName = "COPA", Country = "Spain",
                Division = 0, Tier = 0, GroupIndex = 0, Type = TypeCup,
                TeamCount = 82, Promotions = 0, Relegations = 0, EuropeSlots = 0, Logo = "copa_del_rey"
            },
            new LeagueDataDB
            {
                Id = 9, Name = "Supercopa de España", ShortName = "SUPERCOPA", Country = "Spain",
                Division = 0, Tier = 0, GroupIndex = 0, Type = TypeSuperCup,
                TeamCount = 4, Promotions = 0, Relegations = 0, EuropeSlots = 0, Logo = "supercopa"
            },
        };
    }

    /// <summary>Competiciones de liga (las que tienen equipos asignados).</summary>
    public static bool IsLeague(LeagueDataDB league)
        => league != null && league.Type == TypeLeague;
}
