using Kassabuch.Models;

namespace Kassabuch.Data;

public static class KassabuchSeeder
{
    public static KassabuchDatei ErstelleBeispieldaten()
    {
        return new KassabuchDatei
        {
            Kategorien =
            [
                new Kategorie { Id = 1, Name = "Gehalt", Farbe = "#52734d" },
                new Kategorie { Id = 2, Name = "Vermietung", Farbe = "#3f6f73" },
                new Kategorie { Id = 3, Name = "Miete", Farbe = "#9a5b4f" },
                new Kategorie { Id = 4, Name = "Lebensmittel", Farbe = "#b28a3e" }
            ],
            Kassenbuchungen =
            [
                new Kassenbuchung { Id = 1, Belegnummer = "A-001", Datum = new DateTime(2026, 8, 3), Buchungstext = "Monatsgehalt", Einnahme = 2350m, KategorieId = 1 },
                new Kassenbuchung { Id = 2, Belegnummer = "A-002", Datum = new DateTime(2026, 8, 10), Buchungstext = "Garagenmiete", Einnahme = 240m, KategorieId = 2 },
                new Kassenbuchung { Id = 3, Belegnummer = "B-003", Datum = new DateTime(2026, 8, 15), Buchungstext = "Wohnungsmiete", Ausgabe = 820m, KategorieId = 3 },
                new Kassenbuchung { Id = 4, Belegnummer = "B-004", Datum = new DateTime(2026, 8, 18), Buchungstext = "Wocheneinkauf", Ausgabe = 76.40m, KategorieId = 4 }
            ]
        };
    }
}
