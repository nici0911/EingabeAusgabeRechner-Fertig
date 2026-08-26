namespace EingabeAusgabeRechner.Models;

public class StatistikFilter
{
    public DateTime? Von { get; set; }
    public DateTime? Bis { get; set; }
    public int? KategorieId { get; set; }
    public string DifferenzArt { get; set; } = "alle";
}
