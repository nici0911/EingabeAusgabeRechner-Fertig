// Pro Buchung darf nur Einnahme oder Ausgabe befüllt sein.
document.querySelectorAll(".amount-grid").forEach(gruppe => {
    const einnahme = gruppe.querySelector(".amount-income");
    const ausgabe = gruppe.querySelector(".amount-expense");

    einnahme?.addEventListener("input", () => {
        if (Number(einnahme.value) > 0) ausgabe.value = "0";
    });
    ausgabe?.addEventListener("input", () => {
        if (Number(ausgabe.value) > 0) einnahme.value = "0";
    });
});
