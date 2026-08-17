using Kassabuch.Data;
using Kassabuch.Models;
using Microsoft.EntityFrameworkCore;

namespace Kassabuch.Services;

public class KassabuchService(KassabuchDbContext dbContext) : IKassabuchService
{
    public async Task<KassabuchViewModel> LadeAsync(int? monat, int? jahr)
    {
        var alleBuchungen = await dbContext.Kassenbuchungen
            .AsNoTracking()
            .OrderBy(b => b.Datum)
            .ThenBy(b => b.Id)
            .ToListAsync();

        var zeilen = new List<KassabuchZeile>();
        decimal saldo = 0;

        // Der Saldo wird chronologisch aus allen früheren Belegen fortgeschrieben.
        foreach (var buchung in alleBuchungen)
        {
            saldo += buchung.Einnahme - buchung.Ausgabe;
            var passtZumFilter = (!monat.HasValue || buchung.Datum.Month == monat.Value)
                && (!jahr.HasValue || buchung.Datum.Year == jahr.Value);

            if (passtZumFilter)
            {
                zeilen.Add(new KassabuchZeile { Buchung = buchung, LaufenderSaldo = saldo });
            }
        }

        return new KassabuchViewModel
        {
            Zeilen = zeilen.OrderByDescending(z => z.Buchung.Datum).ThenByDescending(z => z.Buchung.Id).ToList(),
            Monat = monat,
            Jahr = jahr,
            SummeEinnahmen = zeilen.Sum(z => z.Buchung.Einnahme),
            SummeAusgaben = zeilen.Sum(z => z.Buchung.Ausgabe),
            Kassenstand = zeilen.Count > 0 ? zeilen[^1].LaufenderSaldo : saldo
        };
    }

    public Task<bool> BelegnummerExistiertAsync(string belegnummer) =>
        dbContext.Kassenbuchungen.AnyAsync(b => b.Belegnummer == belegnummer.Trim());

    public async Task BuchungAnlegenAsync(KassenbuchungEingabe eingabe)
    {
        dbContext.Kassenbuchungen.Add(new Kassenbuchung
        {
            Belegnummer = eingabe.Belegnummer.Trim(),
            Datum = eingabe.Datum.Date,
            Buchungstext = eingabe.Buchungstext.Trim(),
            Einnahme = eingabe.Einnahme,
            Ausgabe = eingabe.Ausgabe
        });
        await dbContext.SaveChangesAsync();
    }

    public async Task BuchungLoeschenAsync(int id)
    {
        var buchung = await dbContext.Kassenbuchungen.FindAsync(id);
        if (buchung is null) return;

        dbContext.Kassenbuchungen.Remove(buchung);
        await dbContext.SaveChangesAsync();
    }
}
