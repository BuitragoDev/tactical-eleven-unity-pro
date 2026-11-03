/// <summary>
/// Huella del seed estático. `EnsureTemplateDb` reconstruye el template cuando la
/// huella guardada no coincide (frescura, no validez: la validez la comprueba
/// <see cref="SeedAudit"/> y el test de EditMode).
///
/// Se actualiza al cambiar competiciones, equipos o el seed de jugadores.
/// </summary>
public static class SeedBaseline
{
    /// <summary>Versión del esquema que espera el seed (ver `DatabaseManager.SchemaVersion`).</summary>
    public const int SchemaVersion = 1;

    /// <summary>
    /// Huella del seed actual. F1 arranca vacía; F2/F3 la fijan con el hash del
    /// contenido (equipos + competiciones + jugadores).
    /// </summary>
    public const string Fingerprint = "f1-empty";
}
