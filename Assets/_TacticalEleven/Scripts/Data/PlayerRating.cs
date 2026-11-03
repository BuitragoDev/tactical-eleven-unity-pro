using System;

/// <summary>
/// FUENTE ÚNICA del `overall` en fútbol. Se calcula como la media de los atributos
/// **relevantes para la demarcación** (los de portero no penalizan a un jugador de
/// campo ni al revés) y se capa por el potencial.
///
/// Cualquier sitio que derive un overall (seed, progresión, cantera, mercado,
/// migraciones) debe llamar aquí; no dupliques la fórmula.
/// </summary>
public static class PlayerRating
{
    /// <summary>Número de atributos que entran en la media de un jugador de campo.</summary>
    public const int OUTFIELD_ATTRIBUTE_COUNT = 36;

    /// <summary>Número de atributos que entran en la media de un portero.</summary>
    public const int GOALKEEPER_ATTRIBUTE_COUNT = 22;

    /// <summary>Overall de un jugador a partir de sus atributos.</summary>
    public static int FromAttributes(PlayerDataDB p)
    {
        if (p == null) return 0;

        bool isGoalkeeper = p.MainPosition == (int)PlayerPosition.GK;
        int sum = 0;
        int count = 0;

        // Físicos (8): solo para jugadores de campo. En el portero el seed los genera
        // muy por debajo del nivel (no definen su calidad) y sesgarían el overall.
        if (!isGoalkeeper)
        {
            sum += p.Pace + p.Acceleration + p.Stamina + p.Strength
                 + p.Agility + p.Balance + p.Jumping + p.NaturalFitness;
            count += 8;
        }

        // Mentales (14): comunes a todos.
        sum += p.Aggression + p.Anticipation + p.Bravery + p.Composure
             + p.Concentration + p.Decisions + p.Determination + p.Leadership
             + p.OffTheBall + p.Positioning + p.Teamwork + p.Vision
             + p.WorkRate + p.Consistency;
        count += 14;

        if (isGoalkeeper)
        {
            sum += p.Handling + p.Kicking + p.Reflexes + p.GkPositioning
                 + p.OneOnOnes + p.CommandOfArea + p.RushingOut + p.PenaltySaving;
            count += 8;
        }
        else
        {
            // Técnicos (14): no cuentan para el portero.
            sum += p.Passing + p.Crossing + p.Dribbling + p.Finishing
                 + p.FirstTouch + p.Heading + p.LongShots + p.Marking
                 + p.Penalties + p.FreeKicks + p.Corners + p.Tackling
                 + p.Technique + p.Flair;
            count += 14;
        }

        int average = (int)Math.Round(sum / (double)count);
        return p.Potential > 0 && p.Potential < average ? p.Potential : average;
    }

    /// <summary>
    /// Recalcula el overall del jugador desde sus atributos (y lo devuelve).
    /// Llamar tras mutar atributos: entrenamiento, progresión, cantera, edición.
    /// </summary>
    public static int Recalculate(PlayerDataDB p)
    {
        if (p == null) return 0;
        p.Overall = FromAttributes(p);
        return p.Overall;
    }
}

/// <summary>Atajo de conveniencia: <c>player.RecalculateOverall()</c>.</summary>
public static class PlayerDataExtensions
{
    public static void RecalculateOverall(this PlayerDataDB player)
        => PlayerRating.Recalculate(player);
}
