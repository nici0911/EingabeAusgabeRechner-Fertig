using Kassabuch.Models;
using Kassabuch.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kassabuch.Controllers;

/// <summary>
/// Nimmt Eingaben aus der Oberfläche entgegen und übergibt sie an den Service.
/// </summary>
public class HomeController(IKassabuchService kassabuchService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] KassabuchFilter filter, int? bearbeiten) =>
        View(await kassabuchService.LadeAsync(filter, bearbeiten));

    [HttpGet]
    public async Task<IActionResult> Kategorien() =>
        View(await kassabuchService.LadeAsync(new KassabuchFilter()));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Anlegen([Bind(Prefix = "NeueBuchung")] KassenbuchungEingabe eingabe)
    {
        await PruefeBelegnummerAsync(eingabe.Belegnummer);
        if (!ModelState.IsValid)
        {
            var model = await kassabuchService.LadeAsync(new KassabuchFilter());
            model.NeueBuchung = eingabe;
            return View("Index", model);
        }

        await kassabuchService.BuchungAnlegenAsync(eingabe);
        TempData["Erfolg"] = "Die Buchung wurde gespeichert.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Bearbeiten(int id, [Bind(Prefix = "Bearbeitung")] KassenbuchungEingabe eingabe)
    {
        await PruefeBelegnummerAsync(eingabe.Belegnummer, id);
        if (!ModelState.IsValid)
        {
            var model = await kassabuchService.LadeAsync(new KassabuchFilter(), id);
            model.Bearbeitung = eingabe;
            return View("Index", model);
        }

        if (!await kassabuchService.BuchungBearbeitenAsync(id, eingabe)) return NotFound();
        TempData["Erfolg"] = "Die Buchung wurde geändert.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Loeschen(int id)
    {
        await kassabuchService.BuchungLoeschenAsync(id);
        TempData["Erfolg"] = "Die Buchung wurde gelöscht.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> KategorieAnlegen([Bind(Prefix = "NeueKategorie")] KategorieEingabe eingabe)
    {
        if (!ModelState.IsValid)
        {
            var model = await kassabuchService.LadeAsync(new KassabuchFilter());
            model.NeueKategorie = eingabe;
            return View("Kategorien", model);
        }

        if (await kassabuchService.KategorieAnlegenAsync(eingabe))
            TempData["Erfolg"] = "Die Kategorie wurde angelegt und steht sofort zur Auswahl.";
        else
            TempData["Fehler"] = "Dieser Kategoriename ist bereits vorhanden.";
        return RedirectToAction(nameof(Kategorien));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> KategorieLoeschen(int id)
    {
        if (await kassabuchService.KategorieLoeschenAsync(id))
            TempData["Erfolg"] = "Die Kategorie wurde gelöscht.";
        else
            TempData["Fehler"] = "Die Kategorie wird noch von Buchungen verwendet und kann deshalb nicht gelöscht werden.";

        return RedirectToAction(nameof(Kategorien));
    }

    private async Task PruefeBelegnummerAsync(string belegnummer, int? ausgenommenId = null)
    {
        if (!string.IsNullOrWhiteSpace(belegnummer) &&
            await kassabuchService.BelegnummerExistiertAsync(belegnummer, ausgenommenId))
        {
            ModelState.AddModelError(nameof(KassenbuchungEingabe.Belegnummer), "Diese Belegnummer ist bereits vorhanden.");
        }
    }
}
