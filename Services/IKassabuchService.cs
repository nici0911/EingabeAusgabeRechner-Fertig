using Kassabuch.Models;

namespace Kassabuch.Services;

public interface IKassabuchService
{
    Task<KassabuchViewModel> LadeAsync(int? monat, int? jahr);
    Task<bool> BelegnummerExistiertAsync(string belegnummer);
    Task BuchungAnlegenAsync(KassenbuchungEingabe eingabe);
    Task BuchungLoeschenAsync(int id);
}
