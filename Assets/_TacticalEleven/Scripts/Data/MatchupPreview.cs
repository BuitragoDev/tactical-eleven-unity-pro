using System;

/// <summary>Probabilidades de resultado de un partido (local / empate / visitante).</summary>
public struct MatchupProbabilities
{
    public float HomeWin;
    public float Draw;
    public float AwayWin;

    public bool IsHomeFavorite => HomeWin > AwayWin;
}

/// <summary>
/// F8 — Pronóstico previo de un partido: probabilidades a partir del nivel de los
/// clubes, con ventaja de campo. Sin Unity ni base de datos.
/// </summary>
public static class MatchupPreview
{
    private const int MaxGoals = 8;

    public static MatchupProbabilities For(TeamDataDB home, TeamDataDB away)
    {
        double homeXg = Clamp(1.30 * ((home?.Overall ?? 60) / (double)Math.Max(45f, away?.Overall ?? 60)) * 1.12, 0.2, 4.5);
        double awayXg = Clamp(1.10 * ((away?.Overall ?? 60) / (double)Math.Max(45f, home?.Overall ?? 60)), 0.15, 4.0);

        double homeWin = 0, draw = 0, awayWin = 0;
        double total = 0;

        for (int h = 0; h <= MaxGoals; h++)
        {
            double ph = Poisson(h, homeXg);
            for (int a = 0; a <= MaxGoals; a++)
            {
                double p = ph * Poisson(a, awayXg);
                total += p;

                if (h > a) homeWin += p;
                else if (h == a) draw += p;
                else awayWin += p;
            }
        }

        if (total <= 0) return new MatchupProbabilities { HomeWin = 0.4f, Draw = 0.25f, AwayWin = 0.35f };

        return new MatchupProbabilities
        {
            HomeWin = (float)(homeWin / total),
            Draw = (float)(draw / total),
            AwayWin = (float)(awayWin / total)
        };
    }

    private static double Poisson(int goals, double lambda)
    {
        if (lambda <= 0) return goals == 0 ? 1 : 0;
        double logP = -lambda + goals * Math.Log(lambda) - LogFactorial(goals);
        return Math.Exp(logP);
    }

    private static double LogFactorial(int n)
    {
        double sum = 0;
        for (int i = 2; i <= n; i++) sum += Math.Log(i);
        return sum;
    }

    private static double Clamp(double value, double min, double max)
        => value < min ? min : value > max ? max : value;
}
