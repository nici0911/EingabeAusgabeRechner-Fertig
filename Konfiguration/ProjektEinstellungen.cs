namespace EingabeAusgabeRechner.Konfiguration;

/// <summary>
/// Zentrale Stelle für Datumswerte und Zeichenlimits.
/// </summary>
public static class ProjektEinstellungen
{
    public static readonly DateTime MaxDatum = new(2026, 8, 26);
    public static DateTime StandardDatum => DateTime.Today <= MaxDatum ? DateTime.Today : MaxDatum;

    public const int BeschreibungMaxZeichen = 80;
    public const int KategorieMaxZeichen = 30;
}
