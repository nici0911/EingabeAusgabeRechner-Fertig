# Kassabuch

Eigenständige C#-Webanwendung mit ASP.NET Core MVC und SQLite. Sie führt ein klassisches Kassabuch mit Belegnummer, Einnahme, Ausgabe und automatisch berechnetem laufendem Saldo.

## Start in Visual Studio

1. `Kassabuch.sln` doppelt anklicken.
2. Oben als Startprojekt **Kassabuch** auswählen.
3. Mit `F5` oder dem grünen Startknopf ausführen.

Beim ersten Start wird `kassabuch.db` automatisch angelegt und mit drei Beispielbelegen gefüllt.

## Funktionen

- Belege mit eindeutiger Belegnummer, Datum und Buchungstext erfassen
- Genau eine Einnahme oder Ausgabe pro Beleg
- Laufenden Saldo automatisch und chronologisch berechnen
- Nach Monat und Jahr filtern sowie Filter zurücksetzen
- Belege löschen und Saldo danach neu berechnen
- Dauerhafte Speicherung in einer eigenen SQLite-Datenbank
- Einfach strukturierter, deutsch kommentierter C#-Code

Die laut Angabe benötigten Planungsunterlagen stehen vollständig in [DOKUMENTATION.md](DOKUMENTATION.md).
