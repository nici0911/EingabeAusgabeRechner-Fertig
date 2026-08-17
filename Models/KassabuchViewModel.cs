namespace Kassabuch.Models;

public class KassabuchViewModel
{
    public IReadOnlyList<KassabuchZeile> Zeilen { get; set; } = [];
    public KassenbuchungEingabe NeueBuchung { get; set; } = new();
    public int? Monat { get; set; }
    public int? Jahr { get; set; }
    public decimal SummeEinnahmen { get; set; }
    public decimal SummeAusgaben { get; set; }
    public decimal Kassenstand { get; set; }
}
