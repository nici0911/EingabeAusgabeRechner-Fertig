namespace EingabeAusgabeRechner.Models;

public class KategorieStatistik
{
    public int KategorieId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Farbe { get; set; } = "#6d7a71";
    public int Anzahl { get; set; }
    public decimal Einnahmen { get; set; }
    public decimal Ausgaben { get; set; }
    public decimal Differenz => Einnahmen - Ausgaben;
}
