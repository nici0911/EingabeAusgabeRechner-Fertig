using EingabeAusgabeRechner.Data;
using EingabeAusgabeRechner.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

// HTML-Zahlenfelder senden Dezimalzahlen immer mit Punkt, daher bindet der Server invariant.
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddControllersWithViews();
builder.Services.AddDataProtection().PersistKeysToFileSystem(
    new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, ".dataprotection")));

// SQLite speichert alle Daten lokal in einer Datei im Projektordner.
var datenbankPfad = Path.Combine(builder.Environment.ContentRootPath, "eingabe-ausgabe-rechner.db");
builder.Services.AddDbContext<RechnerDbContext>(optionen =>
    optionen.UseSqlite($"Data Source={datenbankPfad}"));
builder.Services.AddScoped<IRechnerService, RechnerService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseRouting();
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Beim ersten Start werden Datenbank und verständliche Beispieldaten angelegt.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RechnerDbContext>();
    await RechnerSeeder.InitialisiereAsync(db);
}

app.Run();
