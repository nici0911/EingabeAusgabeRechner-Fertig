# Benutzerhandbuch – Kassabuch

## Programm starten

Öffnen Sie `Kassabuch.sln` in Visual Studio und drücken Sie `F5`. Beim ersten Start wird die lokale Datenbank automatisch erstellt. Eine Internetverbindung ist nicht erforderlich.

## Neue Buchung anlegen

1. Rechts bei **Buchung hinzufügen** eine eindeutige Belegnummer eintragen.
2. Ein Datum bis einschließlich **26.08.2026** wählen.
3. Beschreibung und Kategorie eintragen.
4. Entweder Einnahme oder Ausgabe ausfüllen. Das jeweils andere Feld bleibt null.
5. **Buchung speichern** auswählen.

## Buchung bearbeiten oder löschen

Bei einer Tabellenzeile führt **Bearbeiten** zum vorausgefüllten Formular. Nach der Änderung **Änderungen speichern** auswählen. Mit **Löschen** wird der Eintrag nach einer Sicherheitsfrage entfernt und der Kontostand neu berechnet.

## Kategorien verwalten

Unter **Kategorien** Namen und Farbe auswählen und **Hinzufügen** drücken. Die Kategorie steht sofort im Buchungsformular und im Filter zur Verfügung. Eine bereits verwendete Kategorie kann erst gelöscht werden, wenn keine Buchung mehr darauf verweist.

## Suchen, filtern und sortieren

Im Filter können Text/Belegnummer, Zeitraum, Kategorie sowie Mindest- und Höchstbetrag kombiniert werden. Die Sortierung ist nach Datum oder Betrag auf- und absteigend möglich. **Alle Filter löschen** stellt die Gesamtansicht wieder her.

Die Kennzahlen beziehen sich auf die aktuelle Filterauswahl. Wird nur ein Tag ausgewählt, werden daher auch Durchschnitt und größter Betrag genau für diesen Tag berechnet.

## Berichte

Unter der Tabelle kann zwischen täglicher, wöchentlicher und monatlicher Auswertung gewechselt werden. Der Bericht übernimmt die gesetzten Filter und zeigt Einnahmen, Ausgaben und Ergebnis.

## Häufige Fehlermeldungen

- **Belegnummer bereits vorhanden:** eine andere Belegnummer verwenden.
- **Entweder Einnahme oder Ausgabe:** nur eines der beiden Betragsfelder ausfüllen.
- **Datum darf höchstens ... sein:** ein Datum bis 26.08.2026 wählen.
- **Kategorie wird verwendet:** zugehörige Buchungen zuerst ändern oder löschen.

