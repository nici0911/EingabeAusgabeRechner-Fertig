using System.ComponentModel.DataAnnotations;
using EingabeAusgabeRechner.Konfiguration;

namespace EingabeAusgabeRechner.Models;

public class Buchung
{
    public int Id { get; set; }

    public DateTime Datum { get; set; }

    [Required]
    [StringLength(ProjektEinstellungen.BeschreibungMaxZeichen)]
    public string Buchungstext { get; set; } = string.Empty;

    public decimal Einnahme { get; set; }
    public decimal Ausgabe { get; set; }

    public int KategorieId { get; set; }
    public Kategorie Kategorie { get; set; } = null!;
}
