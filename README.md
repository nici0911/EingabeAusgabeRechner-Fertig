# Kassabuch – fertige Version

Diese Version erfüllt alle verpflichtenden Punkte der Angabe. Die optionale Mehrbenutzer-Anmeldung ist als freiwillige Erweiterung gekennzeichnet und nicht Teil dieser Variante.

## Start in Visual Studio

1. `Kassabuch.sln` in Visual Studio öffnen.
2. Falls nötig die NuGet-Pakete wiederherstellen lassen.
3. Mit `F5` oder dem Startknopf ausführen.

Die lokale SQLite-Datenbank `kassabuch-schulprojekt.db` entsteht automatisch beim ersten Start. Es sind verständliche Beispieldaten enthalten.

## Wichtige Dateien

- `Konfiguration/PruefungsEinstellungen.cs`: Prüfungstag und Zeichenlimits ändern.
- `Controllers/HomeController.cs`: verarbeitet Formulare und Navigation.
- `Services/KassabuchService.cs`: Filter, Berechnungen, Berichte und Datenzugriff.
- `DOKUMENTATION.docx`: technische Dokumentation und Anforderungscheck als Word-Datei.
- `BENUTZERHANDBUCH.docx`: Bedienung Schritt für Schritt als Word-Datei.
- `docs/skizze.svg`: geplante Ansicht als händisch wirkende Skizze.

## Technische Daten

- C# / ASP.NET Core MVC, .NET 9
- Entity Framework Core
- SQLite, lokal und ohne Internet nutzbar
- eigenes HTML/Razor und eigenes CSS
- kein Bootstrap, kein jQuery und kein CDN
