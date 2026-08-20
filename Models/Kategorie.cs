using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Kassabuch.Konfiguration;

namespace Kassabuch.Models;

public class Kategorie
{
    public int Id { get; set; }

    [Required]
    [StringLength(PruefungsEinstellungen.KategorieMaxZeichen)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(7)]
    public string Farbe { get; set; } = "#6d7a71";

    [JsonIgnore]
    public ICollection<Kassenbuchung> Buchungen { get; set; } = new List<Kassenbuchung>();
}
