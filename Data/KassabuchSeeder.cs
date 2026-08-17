using Kassabuch.Models;
using Microsoft.EntityFrameworkCore;

namespace Kassabuch.Data;

public static class KassabuchSeeder
{
    public static async Task InitialisiereAsync(KassabuchDbContext dbContext)
    {
        await dbContext.Database.EnsureCreatedAsync();
        if (await dbContext.Kassenbuchungen.AnyAsync()) return;

        dbContext.Kassenbuchungen.AddRange(
            new Kassenbuchung { Belegnummer = "A-001", Datum = DateTime.Today.AddDays(-8), Buchungstext = "Bareinlage", Einnahme = 500m },
            new Kassenbuchung { Belegnummer = "B-002", Datum = DateTime.Today.AddDays(-5), Buchungstext = "Büromaterial", Ausgabe = 46.90m },
            new Kassenbuchung { Belegnummer = "B-003", Datum = DateTime.Today.AddDays(-2), Buchungstext = "Portokosten", Ausgabe = 12.40m });
        await dbContext.SaveChangesAsync();
    }
}
