using System.ComponentModel.DataAnnotations;
using Kassabuch.Konfiguration;

namespace Kassabuch.Models;

public class Kassenbuchung
{
    public int Id { get; set; }

    [Required]
    [StringLength(ProjektEinstellungen.BelegnummerMaxZeichen)]
    public string Belegnummer { get; set; } = string.Empty;

    public DateTime Datum { get; set; }

    [Required]
    [StringLength(ProjektEinstellungen.BeschreibungMaxZeichen)]
    public string Buchungstext { get; set; } = string.Empty;

    public decimal Einnahme { get; set; }
    public decimal Ausgabe { get; set; }

    public int KategorieId { get; set; }
    public Kategorie Kategorie { get; set; } = null!;
}
