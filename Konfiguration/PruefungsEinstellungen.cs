namespace Kassabuch.Konfiguration;

/// <summary>
/// Zentrale Stelle für Werte, die vor der Prüfung leicht angepasst werden können.
/// </summary>
public static class PruefungsEinstellungen
{
    public static readonly DateTime MaxDatum = new(2026, 8, 26);
    public static DateTime StandardDatum => DateTime.Today <= MaxDatum ? DateTime.Today : MaxDatum;

    public const int BelegnummerMaxZeichen = 20;
    public const int BeschreibungMaxZeichen = 80;
    public const int KategorieMaxZeichen = 30;
}
