using Kassabuch.Models;
using Kassabuch.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kassabuch.Controllers;

public class HomeController(IKassabuchService kassabuchService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(int? monat, int? jahr)
    {
        return View(await kassabuchService.LadeAsync(monat, jahr));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Anlegen([Bind(Prefix = "NeueBuchung")] KassenbuchungEingabe eingabe)
    {
        if (!string.IsNullOrWhiteSpace(eingabe.Belegnummer)
            && await kassabuchService.BelegnummerExistiertAsync(eingabe.Belegnummer))
        {
            ModelState.AddModelError("NeueBuchung.Belegnummer", "Diese Belegnummer ist bereits vorhanden.");
        }

        if (!ModelState.IsValid)
        {
            var model = await kassabuchService.LadeAsync(null, null);
            model.NeueBuchung = eingabe;
            return View("Index", model);
        }

        await kassabuchService.BuchungAnlegenAsync(eingabe);
        TempData["Erfolg"] = "Der Beleg wurde ins Kassabuch eingetragen.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Loeschen(int id)
    {
        await kassabuchService.BuchungLoeschenAsync(id);
        TempData["Erfolg"] = "Der Beleg wurde gelöscht und der Saldo neu berechnet.";
        return RedirectToAction(nameof(Index));
    }
}
