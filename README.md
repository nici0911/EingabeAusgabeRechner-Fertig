# Kassabuch – fertige Version

Diese Version erfüllt die verpflichtenden Punkte der Angabe.

## Start in Visual Studio

1. `Kassabuch.sln` in Visual Studio öffnen.
2. Mit `F5` oder dem Startknopf ausführen.

Die lokale SQLite-Datenbank `kassabuch-schulprojekt.db` entsteht automatisch beim ersten Start. Es sind verständliche Beispieldaten enthalten.

Die benötigten NuGet-Pakete liegen im Ordner `offline-packages`. Deshalb kann Visual Studio das Projekt auch ohne Internet wiederherstellen. Benötigt werden Visual Studio mit der ASP.NET-Webentwicklung und das .NET-10-SDK.

## Wichtige Dateien

- `Konfiguration/PruefungsEinstellungen.cs`: Prüfungstag und Zeichenlimits ändern.
- `Controllers/HomeController.cs`: verarbeitet Formulare und Navigation.
- `Services/KassabuchService.cs`: Filter, Berechnungen, Berichte und Datenzugriff.
- `DOKUMENTATION.docx`: technische Dokumentation als Word-Datei.
- `BENUTZERHANDBUCH.docx`: Bedienung Schritt für Schritt als Word-Datei.
- `docs/skizze.svg`: geplante Ansicht als händisch wirkende Skizze.

## Technische Daten

- C# / ASP.NET Core MVC, .NET 10
- lokale SQLite-Datenbank mit Entity Framework Core
- lokale NuGet-Quelle für den Start ohne Internet
- einfaches HTML/Razor und eigenes CSS
- kein Bootstrap, kein jQuery und kein CDN
