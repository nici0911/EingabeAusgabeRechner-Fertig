using EingabeAusgabeRechner.Models;
using Microsoft.EntityFrameworkCore;

namespace EingabeAusgabeRechner.Data;

/// <summary>Beschreibt Tabellen und Beziehungen der lokalen SQLite-Datenbank.</summary>
public class RechnerDbContext(DbContextOptions<RechnerDbContext> options) : DbContext(options)
{
    public DbSet<Buchung> Buchungen => Set<Buchung>();
    public DbSet<Kategorie> Kategorien => Set<Kategorie>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Buchung>().Property(b => b.Einnahme).HasPrecision(12, 2);
        modelBuilder.Entity<Buchung>().Property(b => b.Ausgabe).HasPrecision(12, 2);
        modelBuilder.Entity<Buchung>()
            .HasOne(b => b.Kategorie)
            .WithMany(k => k.Buchungen)
            .HasForeignKey(b => b.KategorieId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Kategorie>().HasIndex(k => k.Name).IsUnique();
    }
}
