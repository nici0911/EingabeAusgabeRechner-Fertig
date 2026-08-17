using System.ComponentModel.DataAnnotations;

namespace Kassabuch.Models;

public class Kassenbuchung
{
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string Belegnummer { get; set; } = string.Empty;

    public DateTime Datum { get; set; }

    [Required]
    [StringLength(80)]
    public string Buchungstext { get; set; } = string.Empty;

    public decimal Einnahme { get; set; }
    public decimal Ausgabe { get; set; }
}
