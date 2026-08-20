# Kassabuch – fertige Version

Diese Version erfüllt alle verpflichtenden Punkte der Angabe. Die optionale Mehrbenutzer-Anmeldung ist als freiwillige Erweiterung gekennzeichnet und nicht Teil dieser Variante.

## Start in Visual Studio

1. `Kassabuch.sln` in Visual Studio öffnen.
2. Mit `F5` oder dem Startknopf ausführen.

Die lokale Datei `kassabuch-daten.json` entsteht automatisch beim ersten Start. Es sind verständliche Beispieldaten enthalten.

Das Projekt benötigt keine NuGet-Pakete und keine Internetverbindung. Benötigt werden nur Visual Studio mit der ASP.NET-Webentwicklung und das .NET-10-SDK.

## Wichtige Dateien

- `Konfiguration/PruefungsEinstellungen.cs`: Prüfungstag und Zeichenlimits ändern.
- `Controllers/HomeController.cs`: verarbeitet Formulare und Navigation.
- `Services/KassabuchService.cs`: Filter, Berechnungen, Berichte und Datenzugriff.
- `DOKUMENTATION.docx`: technische Dokumentation und Anforderungscheck als Word-Datei.
- `BENUTZERHANDBUCH.docx`: Bedienung Schritt für Schritt als Word-Datei.
- `docs/skizze.svg`: geplante Ansicht als händisch wirkende Skizze.

## Technische Daten

- C# / ASP.NET Core MVC, .NET 10
- lokale JSON-Datei mit `System.Text.Json`
- keine externen Pakete oder Online-Quellen
- einfaches HTML/Razor und eigenes CSS
- kein Bootstrap, kein jQuery und kein CDN
