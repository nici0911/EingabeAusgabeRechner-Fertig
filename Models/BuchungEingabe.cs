using System.ComponentModel.DataAnnotations;
using EingabeAusgabeRechner.Konfiguration;

namespace EingabeAusgabeRechner.Models;

public class BuchungEingabe : IValidatableObject
{
    [Required(ErrorMessage = "Bitte ein Datum auswählen.")]
    [DataType(DataType.Date)]
    public DateTime Datum { get; set; } = ProjektEinstellungen.StandardDatum;

    [Required(ErrorMessage = "Bitte einen Buchungstext eingeben.")]
    [StringLength(ProjektEinstellungen.BeschreibungMaxZeichen, ErrorMessage = "Der Buchungstext darf höchstens 80 Zeichen lang sein.")]
    [Display(Name = "Buchungstext")]
    public string Buchungstext { get; set; } = string.Empty;

    [Range(typeof(decimal), "0", "999999999", ErrorMessage = "Bitte einen gültigen Betrag eingeben.")]
    public decimal Einnahme { get; set; }

    [Range(typeof(decimal), "0", "999999999", ErrorMessage = "Bitte einen gültigen Betrag eingeben.")]
    public decimal Ausgabe { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Bitte eine Kategorie auswählen.")]
    [Display(Name = "Kategorie")]
    public int KategorieId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Pro Zeile muss genau eine Seite befüllt sein: Einnahme oder Ausgabe.
        if ((Einnahme <= 0 && Ausgabe <= 0) || (Einnahme > 0 && Ausgabe > 0))
        {
            yield return new ValidationResult(
                "Bitte entweder eine Einnahme oder eine Ausgabe größer als 0 eingeben.",
                [nameof(Einnahme), nameof(Ausgabe)]);
        }

        if (Datum.Date > ProjektEinstellungen.MaxDatum)
        {
            yield return new ValidationResult(
                $"Das Datum darf höchstens der {ProjektEinstellungen.MaxDatum:dd.MM.yyyy} sein.",
                [nameof(Datum)]);
        }
    }
}
