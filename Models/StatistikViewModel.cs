namespace EingabeAusgabeRechner.Models;

public class StatistikViewModel
{
    public StatistikFilter Filter { get; set; } = new();
    public IReadOnlyList<Kategorie> Kategorien { get; set; } = [];
    public IReadOnlyList<KategorieStatistik> KategorieWerte { get; set; } = [];
    public int AnzahlBuchungen { get; set; }
    public decimal SummeEinnahmen { get; set; }
    public decimal SummeAusgaben { get; set; }
    public decimal Differenz => SummeEinnahmen - SummeAusgaben;
}
