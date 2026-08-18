using Kassabuch.Models;
using Microsoft.EntityFrameworkCore;

namespace Kassabuch.Data;

public static class KassabuchSeeder
{
    public static async Task InitialisiereAsync(KassabuchDbContext dbContext)
    {
        await dbContext.Database.EnsureCreatedAsync();
        if (await dbContext.Kassenbuchungen.AnyAsync()) return;

        var gehalt = new Kategorie { Name = "Gehalt", Farbe = "#52734d" };
        var vermietung = new Kategorie { Name = "Vermietung", Farbe = "#3f6f73" };
        var miete = new Kategorie { Name = "Miete", Farbe = "#9a5b4f" };
        var lebensmittel = new Kategorie { Name = "Lebensmittel", Farbe = "#b28a3e" };
        dbContext.Kategorien.AddRange(gehalt, vermietung, miete, lebensmittel);
        await dbContext.SaveChangesAsync();

        dbContext.Kassenbuchungen.AddRange(
            new Kassenbuchung { Belegnummer = "A-001", Datum = new DateTime(2026, 8, 3), Buchungstext = "Monatsgehalt", Einnahme = 2350m, KategorieId = gehalt.Id },
            new Kassenbuchung { Belegnummer = "A-002", Datum = new DateTime(2026, 8, 10), Buchungstext = "Garagenmiete", Einnahme = 240m, KategorieId = vermietung.Id },
            new Kassenbuchung { Belegnummer = "B-003", Datum = new DateTime(2026, 8, 15), Buchungstext = "Wohnungsmiete", Ausgabe = 820m, KategorieId = miete.Id },
            new Kassenbuchung { Belegnummer = "B-004", Datum = new DateTime(2026, 8, 18), Buchungstext = "Wocheneinkauf", Ausgabe = 76.40m, KategorieId = lebensmittel.Id });
        await dbContext.SaveChangesAsync();
    }
}
