using System.Globalization;
using EingabeAusgabeRechner.Data;
using EingabeAusgabeRechner.Models;
using Microsoft.EntityFrameworkCore;

namespace EingabeAusgabeRechner.Services;

/// <summary>Enthält Filter, Sortierung, Berichte, Statistik und Datenbankzugriff.</summary>
public class RechnerService(RechnerDbContext db) : IRechnerService
{
    public async Task<UebersichtViewModel> LadeAsync(BuchungsFilter filter, int? bearbeitenId = null)
    {
        var alleBuchungen = await LadeBuchungenAsync();
        IEnumerable<Buchung> gefiltert = alleBuchungen;

        if (!string.IsNullOrWhiteSpace(filter.Suche))
        {
            var suche = filter.Suche.Trim();
            gefiltert = gefiltert.Where(b =>
                b.Buchungstext.Contains(suche, StringComparison.OrdinalIgnoreCase));
        }

        gefiltert = Filtere(
            gefiltert,
            filter.Von,
            filter.Bis,
            filter.KategorieId,
            filter.DifferenzArt);

        if (filter.Mindestbetrag.HasValue)
            gefiltert = gefiltert.Where(b => Betrag(b) >= filter.Mindestbetrag.Value);
        if (filter.Hoechstbetrag.HasValue)
            gefiltert = gefiltert.Where(b => Betrag(b) <= filter.Hoechstbetrag.Value);

        var liste = Sortiere(gefiltert, filter.Sortierung).ToList();
        var einnahmen = liste.Where(b => b.Einnahme > 0).Select(b => b.Einnahme).ToList();
        var ausgaben = liste.Where(b => b.Ausgabe > 0).Select(b => b.Ausgabe).ToList();

        BuchungEingabe? bearbeitung = null;
        if (bearbeitenId.HasValue)
        {
            var buchung = alleBuchungen.FirstOrDefault(b => b.Id == bearbeitenId.Value);
            if (buchung is not null) bearbeitung = ZuEingabe(buchung);
        }

        return new UebersichtViewModel
        {
            Buchungen = liste,
            Kategorien = await LadeKategorienAsync(),
            Bericht = ErstelleBericht(liste, filter.Bericht),
            Filter = filter,
            BearbeitenId = bearbeitung is null ? null : bearbeitenId,
            Bearbeitung = bearbeitung,
            SummeEinnahmen = einnahmen.Sum(),
            SummeAusgaben = ausgaben.Sum(),
            DurchschnittEinnahmen = einnahmen.Count == 0 ? 0 : einnahmen.Average(),
            DurchschnittAusgaben = ausgaben.Count == 0 ? 0 : ausgaben.Average(),
            GroessteEinnahme = einnahmen.DefaultIfEmpty(0).Max(),
            GroessteAusgabe = ausgaben.DefaultIfEmpty(0).Max()
        };
    }

    public async Task<StatistikViewModel> LadeStatistikAsync(StatistikFilter filter)
    {
        var alleBuchungen = await LadeBuchungenAsync();
        var buchungen = Filtere(
                alleBuchungen,
                filter.Von,
                filter.Bis,
                filter.KategorieId,
                filter.DifferenzArt)
            .ToList();

        return new StatistikViewModel
        {
            Filter = filter,
            Kategorien = await LadeKategorienAsync(),
            AnzahlBuchungen = buchungen.Count,
            SummeEinnahmen = buchungen.Sum(b => b.Einnahme),
            SummeAusgaben = buchungen.Sum(b => b.Ausgabe),
            KategorieWerte = buchungen
                .GroupBy(b => new { b.KategorieId, b.Kategorie.Name, b.Kategorie.Farbe })
                .Select(gruppe => new KategorieStatistik
                {
                    KategorieId = gruppe.Key.KategorieId,
                    Name = gruppe.Key.Name,
                    Farbe = gruppe.Key.Farbe,
                    Anzahl = gruppe.Count(),
                    Einnahmen = gruppe.Sum(b => b.Einnahme),
                    Ausgaben = gruppe.Sum(b => b.Ausgabe)
                })
                .OrderByDescending(wert => wert.Einnahmen + wert.Ausgaben)
                .ThenBy(wert => wert.Name)
                .ToList()
        };
    }

    public async Task BuchungAnlegenAsync(BuchungEingabe eingabe)
    {
        var buchung = new Buchung();
        Uebertrage(eingabe, buchung);
        db.Buchungen.Add(buchung);
        await db.SaveChangesAsync();
    }

    public async Task<bool> BuchungBearbeitenAsync(int id, BuchungEingabe eingabe)
    {
        var buchung = await db.Buchungen.FindAsync(id);
        if (buchung is null) return false;
        Uebertrage(eingabe, buchung);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task BuchungLoeschenAsync(int id)
    {
        var buchung = await db.Buchungen.FindAsync(id);
        if (buchung is null) return;
        db.Buchungen.Remove(buchung);
        await db.SaveChangesAsync();
    }

    public async Task<bool> KategorieAnlegenAsync(KategorieEingabe eingabe)
    {
        var name = eingabe.Name.Trim();
        var vergleich = name.ToLower();
        if (await db.Kategorien.AnyAsync(k => k.Name.ToLower() == vergleich)) return false;

        db.Kategorien.Add(new Kategorie { Name = name, Farbe = eingabe.Farbe });
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> KategorieLoeschenAsync(int id)
    {
        if (await db.Buchungen.AnyAsync(b => b.KategorieId == id)) return false;
        var kategorie = await db.Kategorien.FindAsync(id);
        if (kategorie is null) return true;
        db.Kategorien.Remove(kategorie);
        await db.SaveChangesAsync();
        return true;
    }

    private Task<List<Buchung>> LadeBuchungenAsync() => db.Buchungen
        .Include(b => b.Kategorie)
        .AsNoTracking()
        .OrderBy(b => b.Datum)
        .ThenBy(b => b.Id)
        .ToListAsync();

    private Task<List<Kategorie>> LadeKategorienAsync() => db.Kategorien
        .AsNoTracking()
        .OrderBy(k => k.Name)
        .ToListAsync();

    private static IEnumerable<Buchung> Filtere(
        IEnumerable<Buchung> buchungen,
        DateTime? von,
        DateTime? bis,
        int? kategorieId,
        string differenzArt)
    {
        if (von.HasValue) buchungen = buchungen.Where(b => b.Datum.Date >= von.Value.Date);
        if (bis.HasValue) buchungen = buchungen.Where(b => b.Datum.Date <= bis.Value.Date);
        if (kategorieId.HasValue) buchungen = buchungen.Where(b => b.KategorieId == kategorieId.Value);

        buchungen = differenzArt switch
        {
            "positiv" => buchungen.Where(b => b.Einnahme - b.Ausgabe > 0),
            "negativ" => buchungen.Where(b => b.Einnahme - b.Ausgabe < 0),
            _ => buchungen
        };

        return buchungen;
    }

    private static decimal Betrag(Buchung buchung) =>
        buchung.Einnahme > 0 ? buchung.Einnahme : buchung.Ausgabe;

    private static IEnumerable<Buchung> Sortiere(IEnumerable<Buchung> buchungen, string sortierung) =>
        sortierung switch
        {
            "datum_auf" => buchungen.OrderBy(b => b.Datum).ThenBy(b => b.Id),
            "betrag_auf" => buchungen.OrderBy(Betrag).ThenBy(b => b.Datum),
            "betrag_ab" => buchungen.OrderByDescending(Betrag).ThenByDescending(b => b.Datum),
            _ => buchungen.OrderByDescending(b => b.Datum).ThenByDescending(b => b.Id)
        };

    private static IReadOnlyList<Berichtszeile> ErstelleBericht(IReadOnlyList<Buchung> buchungen, string art)
    {
        IEnumerable<IGrouping<string, Buchung>> gruppen = art switch
        {
            "tag" => buchungen.GroupBy(b => b.Datum.ToString("dd.MM.yyyy")),
            "woche" => buchungen.GroupBy(b => $"KW {ISOWeek.GetWeekOfYear(b.Datum)}, {ISOWeek.GetYear(b.Datum)}"),
            _ => buchungen.GroupBy(b => b.Datum.ToString("MMMM yyyy", CultureInfo.GetCultureInfo("de-AT")))
        };

        return gruppen.Select(gruppe => new Berichtszeile
        {
            Zeitraum = gruppe.Key,
            Einnahmen = gruppe.Sum(b => b.Einnahme),
            Ausgaben = gruppe.Sum(b => b.Ausgabe)
        }).ToList();
    }

    private static void Uebertrage(BuchungEingabe eingabe, Buchung buchung)
    {
        buchung.Datum = eingabe.Datum.Date;
        buchung.Buchungstext = eingabe.Buchungstext.Trim();
        buchung.Einnahme = eingabe.Einnahme;
        buchung.Ausgabe = eingabe.Ausgabe;
        buchung.KategorieId = eingabe.KategorieId;
    }

    private static BuchungEingabe ZuEingabe(Buchung buchung) => new()
    {
        Datum = buchung.Datum,
        Buchungstext = buchung.Buchungstext,
        Einnahme = buchung.Einnahme,
        Ausgabe = buchung.Ausgabe,
        KategorieId = buchung.KategorieId
    };
}
