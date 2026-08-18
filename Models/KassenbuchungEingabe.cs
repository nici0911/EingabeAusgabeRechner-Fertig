using System.ComponentModel.DataAnnotations;
using Kassabuch.Konfiguration;

namespace Kassabuch.Models;

public class KassenbuchungEingabe : IValidatableObject
{
    [Required(ErrorMessage = "Bitte eine Belegnummer eingeben.")]
    [StringLength(PruefungsEinstellungen.BelegnummerMaxZeichen, ErrorMessage = "Die Belegnummer darf höchstens 20 Zeichen lang sein.")]
    [Display(Name = "Belegnummer")]
    public string Belegnummer { get; set; } = string.Empty;

    [Required(ErrorMessage = "Bitte ein Datum auswählen.")]
    [DataType(DataType.Date)]
    public DateTime Datum { get; set; } = PruefungsEinstellungen.StandardDatum;

    [Required(ErrorMessage = "Bitte einen Buchungstext eingeben.")]
    [StringLength(PruefungsEinstellungen.BeschreibungMaxZeichen, ErrorMessage = "Der Buchungstext darf höchstens 80 Zeichen lang sein.")]
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

        if (Datum.Date > PruefungsEinstellungen.MaxDatum)
        {
            yield return new ValidationResult(
                $"Das Datum darf höchstens der {PruefungsEinstellungen.MaxDatum:dd.MM.yyyy} sein.",
                [nameof(Datum)]);
        }
    }
}
