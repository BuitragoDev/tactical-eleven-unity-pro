using System;
using System.Collections.Generic;

/// <summary>
/// F8 — Formaciones de fútbol y compatibilidad de un jugador con cada puesto.
/// Clase pura: define los 11 huecos de cada sistema y cuánto penaliza jugar fuera
/// de la demarcación natural.
/// </summary>
public static class FormationRules
{
    public const string F442 = "4-4-2";
    public const string F433 = "4-3-3";
    public const string F4231 = "4-2-3-1";
    public const string F352 = "3-5-2";
    public const string F532 = "5-3-2";

    /// <summary>Orden estable: el índice del `TeamDataDB.Formation` apunta aquí.</summary>
    public static readonly string[] All = { F442, F433, F4231, F352, F532 };

    private static readonly Dictionary<string, PlayerPosition[]> SlotsByFormation = new()
    {
        {
            F442, new[]
            {
                PlayerPosition.GK,
                PlayerPosition.LB, PlayerPosition.CB, PlayerPosition.CB, PlayerPosition.RB,
                PlayerPosition.LM, PlayerPosition.CM, PlayerPosition.CM, PlayerPosition.RM,
                PlayerPosition.ST, PlayerPosition.ST
            }
        },
        {
            F433, new[]
            {
                PlayerPosition.GK,
                PlayerPosition.LB, PlayerPosition.CB, PlayerPosition.CB, PlayerPosition.RB,
                PlayerPosition.CDM, PlayerPosition.CM, PlayerPosition.CM,
                PlayerPosition.LW, PlayerPosition.ST, PlayerPosition.RW
            }
        },
        {
            F4231, new[]
            {
                PlayerPosition.GK,
                PlayerPosition.LB, PlayerPosition.CB, PlayerPosition.CB, PlayerPosition.RB,
                PlayerPosition.CDM, PlayerPosition.CDM, PlayerPosition.CAM,
                PlayerPosition.LW, PlayerPosition.ST, PlayerPosition.RW
            }
        },
        {
            F352, new[]
            {
                PlayerPosition.GK,
                PlayerPosition.CB, PlayerPosition.CB, PlayerPosition.CB,
                PlayerPosition.LWB, PlayerPosition.CM, PlayerPosition.CM, PlayerPosition.CM, PlayerPosition.RWB,
                PlayerPosition.ST, PlayerPosition.ST
            }
        },
        {
            F532, new[]
            {
                PlayerPosition.GK,
                PlayerPosition.LWB, PlayerPosition.CB, PlayerPosition.CB, PlayerPosition.CB, PlayerPosition.RWB,
                PlayerPosition.CM, PlayerPosition.CM, PlayerPosition.CM,
                PlayerPosition.ST, PlayerPosition.ST
            }
        }
    };

    /// <summary>Formación por defecto si el club no tiene una válida.</summary>
    public const string Default = F433;

    public static string FromIndex(int index)
        => index >= 0 && index < All.Length ? All[index] : Default;

    public static PlayerPosition[] Slots(string formation)
        => SlotsByFormation.TryGetValue(formation ?? "", out var slots)
            ? slots
            : SlotsByFormation[Default];

    /// <summary>
    /// Rendimiento relativo (0..1) de un jugador en un puesto. 1 = puesto natural;
    /// 0,9 = misma línea; 0,8 = línea contigua; 0,45 = portero fuera de la portería.
    /// </summary>
    public static float Compatibility(PlayerPosition actual, PlayerPosition slot)
    {
        if (actual == slot) return 1f;

        if (actual == PlayerPosition.GK || slot == PlayerPosition.GK) return 0.45f;

        int actualLine = PlayerPositionNames.LineGroup((int)actual);
        int slotLine = PlayerPositionNames.LineGroup((int)slot);

        if (actualLine == slotLine) return Related(actual, slot) ? 0.95f : 0.9f;

        return Math.Abs(actualLine - slotLine) == 1 ? 0.8f : 0.65f;
    }

    /// <summary>Parejas de puestos que se sienten cómodas entre sí.</summary>
    private static bool Related(PlayerPosition a, PlayerPosition b)
    {
        if (IsWing(a) && IsWing(b)) return true;
        if (IsFullback(a) && IsFullback(b)) return true;
        if (IsCentreBack(a) && IsCentreBack(b)) return true;
        if (IsCentralMid(a) && IsCentralMid(b)) return true;
        if (IsStriker(a) && IsStriker(b)) return true;
        return false;
    }

    private static bool IsWing(PlayerPosition p)
        => p is PlayerPosition.LM or PlayerPosition.RM or PlayerPosition.LW or PlayerPosition.RW;

    private static bool IsFullback(PlayerPosition p)
        => p is PlayerPosition.LB or PlayerPosition.RB or PlayerPosition.LWB or PlayerPosition.RWB;

    private static bool IsCentreBack(PlayerPosition p) => p == PlayerPosition.CB;

    private static bool IsCentralMid(PlayerPosition p)
        => p is PlayerPosition.CDM or PlayerPosition.CM or PlayerPosition.CAM;

    private static bool IsStriker(PlayerPosition p)
        => p is PlayerPosition.ST or PlayerPosition.CF;
}
