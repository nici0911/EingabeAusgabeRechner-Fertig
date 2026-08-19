using Kassabuch.Data;
using Kassabuch.Services;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

// HTML-Zahlenfelder senden Dezimalzahlen immer mit Punkt, daher bindet der Server invariant.
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Das Kassabuch verwendet eine eigene Datenbank und ist vom zweiten Projekt getrennt.
// Eigene Datei für die aktuelle Projektversion, damit alte Übungsdatenbanken erhalten bleiben.
var databasePath = Path.Combine(builder.Environment.ContentRootPath, "kassabuch-schulprojekt.db");
builder.Services.AddDbContext<KassabuchDbContext>(options =>
    options.UseSqlite($"Data Source={databasePath}"));
builder.Services.AddScoped<IKassabuchService, KassabuchService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Beim ersten Start werden Datenbank und einige verständliche Demo-Belege angelegt.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<KassabuchDbContext>();
    await KassabuchSeeder.InitialisiereAsync(dbContext);
}

app.Run();
