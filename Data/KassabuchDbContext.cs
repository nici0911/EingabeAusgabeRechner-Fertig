using Kassabuch.Models;

namespace Kassabuch.Data;

/// <summary>
/// Inhalt der lokalen JSON-Datei. Der Dateiname blieb bestehen, damit die
/// Umstellung von der früheren Datenbank leicht nachvollziehbar ist.
/// </summary>
public class KassabuchDatei
{
    public List<Kassenbuchung> Kassenbuchungen { get; set; } = [];
    public List<Kategorie> Kategorien { get; set; } = [];
}
