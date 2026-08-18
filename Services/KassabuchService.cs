using System.Globalization;
using Kassabuch.Data;
using Kassabuch.Models;
using Microsoft.EntityFrameworkCore;

namespace Kassabuch.Services;

/// <summary>
/// Enthält Filter, Sortierung, Berichte und alle Schreibvorgänge des Kassabuchs.
/// </summary>
public class KassabuchService(KassabuchDbContext dbContext) : IKassabuchService
{
    public async Task<KassabuchViewModel> LadeAsync(KassabuchFilter filter, int? bearbeitenId = null)
    {
        var alleBuchungen = await dbContext.Kassenbuchungen
            .Include(b => b.Kategorie)
            .AsNoTracking()
            .OrderBy(b => b.Datum)
            .ThenBy(b => b.Id)
            .ToListAsync();

        // Der tatsächliche Kassenstand wird immer aus allen Buchungen berechnet.
        decimal laufenderSaldo = 0;
        var saldoJeBuchung = new Dictionary<int, decimal>();
        foreach (var buchung in alleBuchungen)
        {
            laufenderSaldo += buchung.Einnahme - buchung.Ausgabe;
            saldoJeBuchung[buchung.Id] = laufenderSaldo;
        }

        IEnumerable<Kassenbuchung> gefiltert = alleBuchungen;
        if (!string.IsNullOrWhiteSpace(filter.Suche))
        {
            var suche = filter.Suche.Trim();
            gefiltert = gefiltert.Where(b =>
                b.Buchungstext.Contains(suche, StringComparison.OrdinalIgnoreCase) ||
                b.Belegnummer.Contains(suche, StringComparison.OrdinalIgnoreCase));
        }
        if (filter.Von.HasValue) gefiltert = gefiltert.Where(b => b.Datum.Date >= filter.Von.Value.Date);
        if (filter.Bis.HasValue) gefiltert = gefiltert.Where(b => b.Datum.Date <= filter.Bis.Value.Date);
        if (filter.KategorieId.HasValue) gefiltert = gefiltert.Where(b => b.KategorieId == filter.KategorieId.Value);
        if (filter.Mindestbetrag.HasValue) gefiltert = gefiltert.Where(b => Betrag(b) >= filter.Mindestbetrag.Value);
        if (filter.Hoechstbetrag.HasValue) gefiltert = gefiltert.Where(b => Betrag(b) <= filter.Hoechstbetrag.Value);

        var liste = Sortiere(gefiltert, filter.Sortierung).ToList();
        var einnahmen = liste.Where(b => b.Einnahme > 0).Select(b => b.Einnahme).ToList();
        var ausgaben = liste.Where(b => b.Ausgabe > 0).Select(b => b.Ausgabe).ToList();

        KassenbuchungEingabe? bearbeitung = null;
        if (bearbeitenId.HasValue)
        {
            var buchung = alleBuchungen.FirstOrDefault(b => b.Id == bearbeitenId.Value);
            if (buchung is not null) bearbeitung = ZuEingabe(buchung);
        }

        return new KassabuchViewModel
        {
            Zeilen = liste.Select(b => new KassabuchZeile
            {
                Buchung = b,
                LaufenderSaldo = saldoJeBuchung[b.Id]
            }).ToList(),
            Kategorien = await dbContext.Kategorien.AsNoTracking().OrderBy(k => k.Name).ToListAsync(),
            Bericht = ErstelleBericht(liste, filter.Bericht),
            Filter = filter,
            BearbeitenId = bearbeitung is null ? null : bearbeitenId,
            Bearbeitung = bearbeitung,
            SummeEinnahmen = einnahmen.Sum(),
            SummeAusgaben = ausgaben.Sum(),
            Kassenstand = laufenderSaldo,
            DurchschnittEinnahmen = einnahmen.Count == 0 ? 0 : einnahmen.Average(),
            DurchschnittAusgaben = ausgaben.Count == 0 ? 0 : ausgaben.Average(),
            GroessteEinnahme = einnahmen.DefaultIfEmpty(0).Max(),
            GroessteAusgabe = ausgaben.DefaultIfEmpty(0).Max()
        };
    }

    public Task<bool> BelegnummerExistiertAsync(string belegnummer, int? ausgenommenId = null) =>
        dbContext.Kassenbuchungen.AnyAsync(b =>
            b.Belegnummer == belegnummer.Trim() && (!ausgenommenId.HasValue || b.Id != ausgenommenId.Value));

    public async Task BuchungAnlegenAsync(KassenbuchungEingabe eingabe)
    {
        var buchung = new Kassenbuchung();
        Uebertrage(eingabe, buchung);
        dbContext.Kassenbuchungen.Add(buchung);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> BuchungBearbeitenAsync(int id, KassenbuchungEingabe eingabe)
    {
        var buchung = await dbContext.Kassenbuchungen.FindAsync(id);
        if (buchung is null) return false;
        Uebertrage(eingabe, buchung);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task BuchungLoeschenAsync(int id)
    {
        var buchung = await dbContext.Kassenbuchungen.FindAsync(id);
        if (buchung is null) return;
        dbContext.Kassenbuchungen.Remove(buchung);
        await dbContext.SaveChangesAsync();
    }

    public async Task KategorieAnlegenAsync(KategorieEingabe eingabe)
    {
        dbContext.Kategorien.Add(new Kategorie { Name = eingabe.Name.Trim(), Farbe = eingabe.Farbe });
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> KategorieLoeschenAsync(int id)
    {
        if (await dbContext.Kassenbuchungen.AnyAsync(b => b.KategorieId == id)) return false;
        var kategorie = await dbContext.Kategorien.FindAsync(id);
        if (kategorie is null) return true;
        dbContext.Kategorien.Remove(kategorie);
        await dbContext.SaveChangesAsync();
        return true;
    }

    private static decimal Betrag(Kassenbuchung b) => b.Einnahme > 0 ? b.Einnahme : b.Ausgabe;

    private static IEnumerable<Kassenbuchung> Sortiere(IEnumerable<Kassenbuchung> buchungen, string sortierung) =>
        sortierung switch
        {
            "datum_auf" => buchungen.OrderBy(b => b.Datum).ThenBy(b => b.Id),
            "betrag_auf" => buchungen.OrderBy(Betrag).ThenBy(b => b.Datum),
            "betrag_ab" => buchungen.OrderByDescending(Betrag).ThenByDescending(b => b.Datum),
            _ => buchungen.OrderByDescending(b => b.Datum).ThenByDescending(b => b.Id)
        };

    private static IReadOnlyList<Berichtszeile> ErstelleBericht(IReadOnlyList<Kassenbuchung> buchungen, string art)
    {
        IEnumerable<IGrouping<string, Kassenbuchung>> gruppen = art switch
        {
            "tag" => buchungen.GroupBy(b => b.Datum.ToString("dd.MM.yyyy")),
            "woche" => buchungen.GroupBy(b => $"KW {ISOWeek.GetWeekOfYear(b.Datum)}, {ISOWeek.GetYear(b.Datum)}"),
            _ => buchungen.GroupBy(b => b.Datum.ToString("MMMM yyyy", CultureInfo.GetCultureInfo("de-AT")))
        };

        return gruppen.Select(g => new Berichtszeile
        {
            Zeitraum = g.Key,
            Einnahmen = g.Sum(b => b.Einnahme),
            Ausgaben = g.Sum(b => b.Ausgabe)
        }).ToList();
    }

    private static void Uebertrage(KassenbuchungEingabe eingabe, Kassenbuchung buchung)
    {
        buchung.Belegnummer = eingabe.Belegnummer.Trim();
        buchung.Datum = eingabe.Datum.Date;
        buchung.Buchungstext = eingabe.Buchungstext.Trim();
        buchung.Einnahme = eingabe.Einnahme;
        buchung.Ausgabe = eingabe.Ausgabe;
        buchung.KategorieId = eingabe.KategorieId;
    }

    private static KassenbuchungEingabe ZuEingabe(Kassenbuchung buchung) => new()
    {
        Belegnummer = buchung.Belegnummer,
        Datum = buchung.Datum,
        Buchungstext = buchung.Buchungstext,
        Einnahme = buchung.Einnahme,
        Ausgabe = buchung.Ausgabe,
        KategorieId = buchung.KategorieId
    };
}
