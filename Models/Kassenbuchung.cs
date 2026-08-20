using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Kassabuch.Konfiguration;

namespace Kassabuch.Models;

public class Kassenbuchung
{
    public int Id { get; set; }

    [Required]
    [StringLength(PruefungsEinstellungen.BelegnummerMaxZeichen)]
    public string Belegnummer { get; set; } = string.Empty;

    public DateTime Datum { get; set; }

    [Required]
    [StringLength(PruefungsEinstellungen.BeschreibungMaxZeichen)]
    public string Buchungstext { get; set; } = string.Empty;

    public decimal Einnahme { get; set; }
    public decimal Ausgabe { get; set; }

    public int KategorieId { get; set; }
    [JsonIgnore]
    public Kategorie Kategorie { get; set; } = null!;
}
