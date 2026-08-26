using EingabeAusgabeRechner.Models;

namespace EingabeAusgabeRechner.Services;

public interface IRechnerService
{
    Task<UebersichtViewModel> LadeAsync(BuchungsFilter filter, int? bearbeitenId = null);
    Task<StatistikViewModel> LadeStatistikAsync(StatistikFilter filter);
    Task BuchungAnlegenAsync(BuchungEingabe eingabe);
    Task<bool> BuchungBearbeitenAsync(int id, BuchungEingabe eingabe);
    Task BuchungLoeschenAsync(int id);
    Task<bool> KategorieAnlegenAsync(KategorieEingabe eingabe);
    Task<bool> KategorieLoeschenAsync(int id);
}
