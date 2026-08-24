# Datenbank-Hinweise

Dieses Projekt verwendet die lokale SQLite-Datenbank `kassabuch.db`. Die Anwendung funktioniert ohne zusätzliche Visual-Studio-Erweiterung. Die Erweiterung wird nur benötigt, wenn Tabellen und Datensätze direkt in Visual Studio angezeigt werden sollen.

## Kurzfassung

1. `Kassabuch.sln` in Visual Studio öffnen.
2. **Ansicht → Server-Explorer** wählen.
3. Im Server-Explorer auf **SQLite/SQL Server Compact Toolbox** klicken.
4. In der Toolbox auf **Add SQLite and SQL Compact connections from current Solution** klicken.
5. **Data Connections → kassabuch.db → Tables** aufklappen.
6. Eine Tabelle mit der rechten Maustaste anklicken und **Edit Top 200 Rows** wählen. Nur ansehen und keine Zellen verändern.

Vorhandene Tabellen: `Kassenbuchungen` und `Kategorien`.

## Voraussetzungen auf dem fremden Gerät

- Visual Studio 2026 mit der Workload **ASP.NET und Webentwicklung**
- .NET 10 SDK
- Für die Tabellenansicht zusätzlich die unten beschriebene SQLite Toolbox

## Einmalige Installation auf einem fremden Gerät

Benötigt wird die Visual-Studio-Erweiterung **SQLite and SQL Server Compact Toolbox**, Version **4.9.65**, Herausgeber **ErikEJ**.

### Offline vom USB-Stick

1. Visual Studio vollständig schließen.
2. Auf dem USB-Stick den Ordner `LAP-Pruefungsprojekte-2026-08-24\Werkzeuge` öffnen.
3. `SQLiteToolbox-4.9.65.vsix` doppelt anklicken.
4. Im VSIX-Installer die vorhandene Visual-Studio-Installation auswählen und **Installieren** beziehungsweise **Ändern** anklicken.
5. Nach Abschluss Visual Studio wieder öffnen.

### Online über Visual Studio

1. In Visual Studio **Erweiterungen → Erweiterungen verwalten** öffnen.
2. Nach `SQLite and SQL Server Compact Toolbox` suchen.
3. Die Erweiterung von **ErikEJ** installieren.
4. Visual Studio schließen, die Installation abschließen und Visual Studio neu starten.

## Falls die Verbindung nicht automatisch erscheint

1. In der SQLite Toolbox **Add SQLite Connection** anklicken.
2. Die Datei `kassabuch.db` aus dem Projektordner auswählen.
3. Danach **Data Connections → kassabuch.db → Tables** öffnen.

Nicht **View Data as Report** verwenden: Diese Funktion verlangt den alten Microsoft Report Viewer 2010. Für die normale Tabellenansicht ist er nicht erforderlich.

Hinweis: Eine Visual-Studio-Erweiterung gehört zur Visual-Studio-Installation des jeweiligen Computers und kann deshalb nicht durch das Projekt automatisch installiert werden. Das Projekt selbst und seine SQLite-Datenbank funktionieren trotzdem ohne diese Erweiterung.

Projekt der Erweiterung und Lizenz: https://github.com/ErikEJ/SqlCeToolbox
