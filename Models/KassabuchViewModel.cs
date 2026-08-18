namespace Kassabuch.Models;

public class KassabuchViewModel
{
    public IReadOnlyList<KassabuchZeile> Zeilen { get; set; } = [];
    public IReadOnlyList<Kategorie> Kategorien { get; set; } = [];
    public IReadOnlyList<Berichtszeile> Bericht { get; set; } = [];
    public KassenbuchungEingabe NeueBuchung { get; set; } = new();
    public KassenbuchungEingabe? Bearbeitung { get; set; }
    public int? BearbeitenId { get; set; }
    public KategorieEingabe NeueKategorie { get; set; } = new();
    public KassabuchFilter Filter { get; set; } = new();
    public decimal SummeEinnahmen { get; set; }
    public decimal SummeAusgaben { get; set; }
    public decimal Kassenstand { get; set; }
    public decimal DurchschnittEinnahmen { get; set; }
    public decimal DurchschnittAusgaben { get; set; }
    public decimal GroessteEinnahme { get; set; }
    public decimal GroessteAusgabe { get; set; }
}
