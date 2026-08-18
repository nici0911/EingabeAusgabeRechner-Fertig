namespace Kassabuch.Models;

public class Berichtszeile
{
    public string Zeitraum { get; set; } = string.Empty;
    public decimal Einnahmen { get; set; }
    public decimal Ausgaben { get; set; }
    public decimal Saldo => Einnahmen - Ausgaben;
}
