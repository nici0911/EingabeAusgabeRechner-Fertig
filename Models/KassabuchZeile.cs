namespace Kassabuch.Models;

public class KassabuchZeile
{
    public required Kassenbuchung Buchung { get; set; }
    public decimal LaufenderSaldo { get; set; }
}
