using System.ComponentModel.DataAnnotations;
using EingabeAusgabeRechner.Konfiguration;

namespace EingabeAusgabeRechner.Models;

public class KategorieEingabe
{
    [Required(ErrorMessage = "Bitte einen Kategorienamen eingeben.")]
    [StringLength(ProjektEinstellungen.KategorieMaxZeichen, ErrorMessage = "Der Name darf höchstens 30 Zeichen lang sein.")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Bitte eine gültige Farbe auswählen.")]
    public string Farbe { get; set; } = "#6d7a71";
}
