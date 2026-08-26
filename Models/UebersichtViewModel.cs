namespace EingabeAusgabeRechner.Models;

public class UebersichtViewModel
{
    public IReadOnlyList<Buchung> Buchungen { get; set; } = [];
    public IReadOnlyList<Kategorie> Kategorien { get; set; } = [];
    public IReadOnlyList<Berichtszeile> Bericht { get; set; } = [];
    public BuchungEingabe NeueBuchung { get; set; } = new();
    public BuchungEingabe? Bearbeitung { get; set; }
    public int? BearbeitenId { get; set; }
    public KategorieEingabe NeueKategorie { get; set; } = new();
    public BuchungsFilter Filter { get; set; } = new();
    public decimal SummeEinnahmen { get; set; }
    public decimal SummeAusgaben { get; set; }
    public decimal Differenz => SummeEinnahmen - SummeAusgaben;
    public decimal DurchschnittEinnahmen { get; set; }
    public decimal DurchschnittAusgaben { get; set; }
    public decimal GroessteEinnahme { get; set; }
    public decimal GroessteAusgabe { get; set; }
}
