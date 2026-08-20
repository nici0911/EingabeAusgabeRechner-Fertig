using Kassabuch.Models;

namespace Kassabuch.Services;

public interface IKassabuchService
{
    Task<KassabuchViewModel> LadeAsync(KassabuchFilter filter, int? bearbeitenId = null);
    Task<bool> BelegnummerExistiertAsync(string belegnummer, int? ausgenommenId = null);
    Task BuchungAnlegenAsync(KassenbuchungEingabe eingabe);
    Task<bool> BuchungBearbeitenAsync(int id, KassenbuchungEingabe eingabe);
    Task BuchungLoeschenAsync(int id);
    Task<bool> KategorieAnlegenAsync(KategorieEingabe eingabe);
    Task<bool> KategorieLoeschenAsync(int id);
}
