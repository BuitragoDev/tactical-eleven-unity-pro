using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Huella del seed estático. Se calcula a partir de las mismas listas que se siembran
/// (competiciones + equipos), así que **cambia sola** cuando cambia el seed y sirve
/// para reconstruir `template.db` sin tocar constantes a mano.
///
/// Es frescura, no validez: la validez la decide <see cref="SeedAudit"/> y los tests.
/// </summary>
public static class SeedBaseline
{
    /// <summary>Versión de esquema que espera el seed (ver `DatabaseManager.SchemaVersion`).</summary>
    public const int SchemaVersion = 1;

    /// <summary>Huella actual del seed (determinista entre ejecuciones).</summary>
    public static string Fingerprint => Compute(CompetitionSeeder.Build(), TeamSeeder.Build());

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

        using var md5 = MD5.Create();
        byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}
