namespace Kassabuch.Models;

public class KassabuchFilter
{
    public string? Suche { get; set; }
    public DateTime? Von { get; set; }
    public DateTime? Bis { get; set; }
    public int? KategorieId { get; set; }
    public decimal? Mindestbetrag { get; set; }
    public decimal? Hoechstbetrag { get; set; }
    public string Sortierung { get; set; } = "datum_ab";
    public string Bericht { get; set; } = "monat";
}
