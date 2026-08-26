using EingabeAusgabeRechner.Models;
using EingabeAusgabeRechner.Services;
using Microsoft.AspNetCore.Mvc;

namespace EingabeAusgabeRechner.Controllers;

/// <summary>
/// Nimmt Eingaben aus der Oberfläche entgegen und übergibt sie an den Service.
/// </summary>
public class HomeController(IRechnerService rechnerService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] BuchungsFilter filter, int? bearbeiten) =>
        View(await rechnerService.LadeAsync(filter, bearbeiten));

    [HttpGet]
    public async Task<IActionResult> Kategorien() =>
        View(await rechnerService.LadeAsync(new BuchungsFilter()));

    [HttpGet]
    public async Task<IActionResult> Statistik([FromQuery] StatistikFilter filter) =>
        View(await rechnerService.LadeStatistikAsync(filter));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Anlegen([Bind(Prefix = "NeueBuchung")] BuchungEingabe eingabe)
    {
        if (!ModelState.IsValid)
        {
            var model = await rechnerService.LadeAsync(new BuchungsFilter());
            model.NeueBuchung = eingabe;
            return View("Index", model);
        }

        await rechnerService.BuchungAnlegenAsync(eingabe);
        TempData["Erfolg"] = "Die Buchung wurde gespeichert.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Bearbeiten(int id, [Bind(Prefix = "Bearbeitung")] BuchungEingabe eingabe)
    {
        if (!ModelState.IsValid)
        {
            var model = await rechnerService.LadeAsync(new BuchungsFilter(), id);
            model.Bearbeitung = eingabe;
            return View("Index", model);
        }

        if (!await rechnerService.BuchungBearbeitenAsync(id, eingabe)) return NotFound();
        TempData["Erfolg"] = "Die Buchung wurde geändert.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Loeschen(int id)
    {
        await rechnerService.BuchungLoeschenAsync(id);
        TempData["Erfolg"] = "Die Buchung wurde gelöscht.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> KategorieAnlegen([Bind(Prefix = "NeueKategorie")] KategorieEingabe eingabe)
    {
        if (!ModelState.IsValid)
        {
            var model = await rechnerService.LadeAsync(new BuchungsFilter());
            model.NeueKategorie = eingabe;
            return View("Kategorien", model);
        }

        if (await rechnerService.KategorieAnlegenAsync(eingabe))
            TempData["Erfolg"] = "Die Kategorie wurde angelegt und steht sofort zur Auswahl.";
        else
            TempData["Fehler"] = "Dieser Kategoriename ist bereits vorhanden.";
        return RedirectToAction(nameof(Kategorien));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> KategorieLoeschen(int id)
    {
        if (await rechnerService.KategorieLoeschenAsync(id))
            TempData["Erfolg"] = "Die Kategorie wurde gelöscht.";
        else
            TempData["Fehler"] = "Die Kategorie wird noch von Buchungen verwendet und kann deshalb nicht gelöscht werden.";

        return RedirectToAction(nameof(Kategorien));
    }
}
