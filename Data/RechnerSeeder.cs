using EingabeAusgabeRechner.Models;
using Microsoft.EntityFrameworkCore;

namespace EingabeAusgabeRechner.Data;

public static class RechnerSeeder
{
    public static async Task InitialisiereAsync(RechnerDbContext db)
    {
        await db.Database.EnsureCreatedAsync();
        if (await db.Buchungen.AnyAsync()) return;

        var gehalt = new Kategorie { Name = "Gehalt", Farbe = "#52734d" };
        var vermietung = new Kategorie { Name = "Vermietung", Farbe = "#3f6f73" };
        var miete = new Kategorie { Name = "Miete", Farbe = "#9a5b4f" };
        var lebensmittel = new Kategorie { Name = "Lebensmittel", Farbe = "#b28a3e" };
        db.Kategorien.AddRange(gehalt, vermietung, miete, lebensmittel);
        await db.SaveChangesAsync();

        db.Buchungen.AddRange(
            new Buchung { Datum = new DateTime(2026, 8, 3), Buchungstext = "Monatsgehalt", Einnahme = 2350m, KategorieId = gehalt.Id },
            new Buchung { Datum = new DateTime(2026, 8, 10), Buchungstext = "Garagenmiete", Einnahme = 240m, KategorieId = vermietung.Id },
            new Buchung { Datum = new DateTime(2026, 8, 15), Buchungstext = "Wohnungsmiete", Ausgabe = 820m, KategorieId = miete.Id },
            new Buchung { Datum = new DateTime(2026, 8, 18), Buchungstext = "Wocheneinkauf", Ausgabe = 76.40m, KategorieId = lebensmittel.Id });
        await db.SaveChangesAsync();
    }
}
