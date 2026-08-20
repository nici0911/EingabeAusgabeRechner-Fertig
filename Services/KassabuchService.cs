using System.Globalization;
using System.Text.Json;
using Kassabuch.Data;
using Kassabuch.Models;

namespace Kassabuch.Services;

/// <summary>
/// Enthält Filter, Sortierung, Berichte und alle Schreibvorgänge des Kassabuchs.
/// Die Daten werden ohne Zusatzpaket in einer lokalen JSON-Datei gespeichert.
/// </summary>
public class KassabuchService : IKassabuchService
{
    private readonly string _dateipfad;
    private readonly SemaphoreSlim _dateisperre = new(1, 1);
    private readonly JsonSerializerOptions _jsonOptionen = new() { WriteIndented = true };

    public KassabuchService(IWebHostEnvironment umgebung)
    {
        _dateipfad = Path.Combine(umgebung.ContentRootPath, "kassabuch-daten.json");
    }

    public async Task<KassabuchViewModel> LadeAsync(KassabuchFilter filter, int? bearbeitenId = null)
    {
        var daten = await DatenLadenAsync();
        VerbindeKategorien(daten);

        var alleBuchungen = daten.Kassenbuchungen
            .OrderBy(b => b.Datum)
            .ThenBy(b => b.Id)
            .ToList();

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
            Kategorien = daten.Kategorien.OrderBy(k => k.Name).ToList(),
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

    public async Task<bool> BelegnummerExistiertAsync(string belegnummer, int? ausgenommenId = null)
    {
        var daten = await DatenLadenAsync();
        return daten.Kassenbuchungen.Any(b =>
            b.Belegnummer.Equals(belegnummer.Trim(), StringComparison.OrdinalIgnoreCase) &&
            (!ausgenommenId.HasValue || b.Id != ausgenommenId.Value));
    }

    public async Task BuchungAnlegenAsync(KassenbuchungEingabe eingabe)
    {
        await _dateisperre.WaitAsync();
        try
        {
            var daten = await DateiLesenOderAnlegenAsync();
            var buchung = new Kassenbuchung
            {
                Id = daten.Kassenbuchungen.Count == 0 ? 1 : daten.Kassenbuchungen.Max(b => b.Id) + 1
            };
            Uebertrage(eingabe, buchung);
            daten.Kassenbuchungen.Add(buchung);
            await DateiSpeichernAsync(daten);
        }
        finally
        {
            _dateisperre.Release();
        }
    }

    public async Task<bool> BuchungBearbeitenAsync(int id, KassenbuchungEingabe eingabe)
    {
        await _dateisperre.WaitAsync();
        try
        {
            var daten = await DateiLesenOderAnlegenAsync();
            var buchung = daten.Kassenbuchungen.FirstOrDefault(b => b.Id == id);
            if (buchung is null) return false;
            Uebertrage(eingabe, buchung);
            await DateiSpeichernAsync(daten);
            return true;
        }
        finally
        {
            _dateisperre.Release();
        }
    }

    public async Task BuchungLoeschenAsync(int id)
    {
        await _dateisperre.WaitAsync();
        try
        {
            var daten = await DateiLesenOderAnlegenAsync();
            daten.Kassenbuchungen.RemoveAll(b => b.Id == id);
            await DateiSpeichernAsync(daten);
        }
        finally
        {
            _dateisperre.Release();
        }
    }

    public async Task<bool> KategorieAnlegenAsync(KategorieEingabe eingabe)
    {
        await _dateisperre.WaitAsync();
        try
        {
            var daten = await DateiLesenOderAnlegenAsync();
            var name = eingabe.Name.Trim();
            if (daten.Kategorien.Any(k => k.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                return false;

            daten.Kategorien.Add(new Kategorie
            {
                Id = daten.Kategorien.Count == 0 ? 1 : daten.Kategorien.Max(k => k.Id) + 1,
                Name = name,
                Farbe = eingabe.Farbe
            });
            await DateiSpeichernAsync(daten);
            return true;
        }
        finally
        {
            _dateisperre.Release();
        }
    }

    public async Task<bool> KategorieLoeschenAsync(int id)
    {
        await _dateisperre.WaitAsync();
        try
        {
            var daten = await DateiLesenOderAnlegenAsync();
            if (daten.Kassenbuchungen.Any(b => b.KategorieId == id)) return false;
            daten.Kategorien.RemoveAll(k => k.Id == id);
            await DateiSpeichernAsync(daten);
            return true;
        }
        finally
        {
            _dateisperre.Release();
        }
    }

    private async Task<KassabuchDatei> DatenLadenAsync()
    {
        await _dateisperre.WaitAsync();
        try
        {
            return await DateiLesenOderAnlegenAsync();
        }
        finally
        {
            _dateisperre.Release();
        }
    }

    private async Task<KassabuchDatei> DateiLesenOderAnlegenAsync()
    {
        if (!File.Exists(_dateipfad))
        {
            var beispieldaten = KassabuchSeeder.ErstelleBeispieldaten();
            await DateiSpeichernAsync(beispieldaten);
            return beispieldaten;
        }

        await using var stream = File.OpenRead(_dateipfad);
        return await JsonSerializer.DeserializeAsync<KassabuchDatei>(stream, _jsonOptionen)
            ?? new KassabuchDatei();
    }

    private async Task DateiSpeichernAsync(KassabuchDatei daten)
    {
        await using var stream = File.Create(_dateipfad);
        await JsonSerializer.SerializeAsync(stream, daten, _jsonOptionen);
    }

    private static void VerbindeKategorien(KassabuchDatei daten)
    {
        foreach (var buchung in daten.Kassenbuchungen)
        {
            buchung.Kategorie = daten.Kategorien.FirstOrDefault(k => k.Id == buchung.KategorieId)
                ?? new Kategorie { Id = buchung.KategorieId, Name = "Unbekannt", Farbe = "#777777" };
        }
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
