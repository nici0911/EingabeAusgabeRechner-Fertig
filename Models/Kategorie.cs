using System.ComponentModel.DataAnnotations;
using EingabeAusgabeRechner.Konfiguration;

namespace EingabeAusgabeRechner.Models;

public class Kategorie
{
    public int Id { get; set; }

    [Required]
    [StringLength(ProjektEinstellungen.KategorieMaxZeichen)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(7)]
    public string Farbe { get; set; } = "#6d7a71";

    public ICollection<Buchung> Buchungen { get; set; } = new List<Buchung>();
}
