using Kassabuch.Services;
using Microsoft.AspNetCore.DataProtection;
using System.Globalization;

// HTML-Zahlenfelder senden Dezimalzahlen immer mit Punkt, daher bindet der Server invariant.
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddControllersWithViews();
builder.Services.AddDataProtection().PersistKeysToFileSystem(
    new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, ".dataprotection")));
builder.Services.AddSingleton<IKassabuchService, KassabuchService>();

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

app.Run();
