# Dokumentation – Kassabuch (fertige Version)

## Projektziel

Das Programm erfasst Einnahmen und Ausgaben in einer lokalen Datenbank. Das Dashboard zeigt den aktuellen Kontostand, gefilterte Summen, Durchschnittswerte und größte Beträge. Die Oberfläche ist bewusst schlicht gehalten, damit die Funktionen bei der Prüfung schnell erklärt werden können.

## Status der Anforderungen

| Anforderung | Umsetzung |
|---|---|
| Dashboard mit aktuellem Kontostand | Erfüllt |
| Tabelle mit allen Transaktionen | Erfüllt |
| Einnahmen und Ausgaben mit Betrag, Datum, Beschreibung und Kategorie | Erfüllt |
| Transaktionen bearbeiten und löschen | Erfüllt |
| Kategorien in frei wählbaren Farben | Erfüllt |
| Suche sowie Filter nach Datum, Kategorie und Betrag | Erfüllt |
| Filter einfach zurücksetzen | Erfüllt |
| Sortierung nach Datum und Betrag | Erfüllt |
| Tägliche, wöchentliche und monatliche Berichte | Erfüllt |
| Lokale Datenbank | Erfüllt (SQLite) |
| Prüfung beim Löschen verwendeter Kategorien | Erfüllt |
| Neu angelegte Kategorie erscheint in Auswahl | Erfüllt |
| Durchschnitt und größter Betrag, auch für gefilterten Tag | Erfüllt |
| Registrierung und Anmeldung | Nicht umgesetzt; laut Angabe optional |

## Datenmodell

```text
Kategorie                         Kassenbuchung
------------------                -----------------------
Id (PK)                  1 ---- n Id (PK)
Name (eindeutig)                  Belegnummer (eindeutig)
Farbe                             Datum
                                  Buchungstext
                                  Einnahme
                                  Ausgabe
                                  KategorieId (FK)
```

Eine Kategorie kann mehreren Buchungen zugeordnet sein. Eine verwendete Kategorie darf nicht gelöscht werden, damit keine ungültigen Buchungen entstehen.

## Wichtigste Anwendungsbestandteile

- **HomeController:** Nimmt Formularwerte entgegen, prüft den Zustand und zeigt passende Erfolg- oder Fehlermeldungen. Der Controller enthält absichtlich keine Berechnungslogik.
- **KassabuchService:** Führt Datenbankzugriffe, Filter, Sortierungen, Statistik, Salden und Berichte aus. Dadurch bleiben Controller und Ansicht übersichtlich.
- **KassabuchDbContext:** Beschreibt Tabellen, Beziehungen, eindeutige Werte und Dezimalfelder für SQLite.
- **Razor-Ansicht:** Zeigt Dashboard, Filter, Tabelle, Berichte und Formulare auf einer Seite an.

## Wo Datum und Zeichenlimits geändert werden

Alle prüfungsrelevanten Grenzwerte stehen in `Konfiguration/PruefungsEinstellungen.cs`:

```csharp
public static readonly DateTime MaxDatum = new(2026, 8, 26);
public const int BelegnummerMaxZeichen = 20;
public const int BeschreibungMaxZeichen = 80;
public const int KategorieMaxZeichen = 30;
```

Das Datum wird im HTML-Feld begrenzt und zusätzlich auf dem Server geprüft. Ein veränderter Browserwert kann die Prüfung daher nicht umgehen.

## Fehlerbehandlung

- Genau eine Seite einer Buchung muss größer als null sein.
- Belegnummern und Kategorienamen sind eindeutig.
- Pflichtfelder und Zeichenlimits werden geprüft.
- Ein Datum nach dem 26.08.2026 wird abgelehnt.
- Verwendete Kategorien können nicht gelöscht werden.
- Leere Filterergebnisse werden verständlich angezeigt.

## Datenhaltung

Die Datei `kassabuch.db` wird beim ersten Start im Projektordner erstellt. `KassabuchSeeder` fügt nur bei leerer Datenbank Beispieldaten hinzu. Alle Beispielbuchungen liegen spätestens am 26.08.2026.

## Ansichtsplanung

Die Skizze befindet sich unter `docs/skizze.svg`. Oben stehen Titel und Kontostand, darunter Kennzahlen und Filter. Die Hauptfläche teilt sich in Tabelle/Bericht links und Eingabe/Kategorien rechts.
