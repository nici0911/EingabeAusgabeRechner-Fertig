using Kassabuch.Models;
using Microsoft.EntityFrameworkCore;

namespace Kassabuch.Data;

/// <summary>Beschreibt Tabellen und Beziehungen der lokalen SQLite-Datenbank.</summary>
public class KassabuchDbContext(DbContextOptions<KassabuchDbContext> options) : DbContext(options)
{
    public DbSet<Kassenbuchung> Kassenbuchungen => Set<Kassenbuchung>();
    public DbSet<Kategorie> Kategorien => Set<Kategorie>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Kassenbuchung>().HasIndex(b => b.Belegnummer).IsUnique();
        modelBuilder.Entity<Kassenbuchung>().Property(b => b.Einnahme).HasPrecision(12, 2);
        modelBuilder.Entity<Kassenbuchung>().Property(b => b.Ausgabe).HasPrecision(12, 2);
        modelBuilder.Entity<Kassenbuchung>()
            .HasOne(b => b.Kategorie)
            .WithMany(k => k.Buchungen)
            .HasForeignKey(b => b.KategorieId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Kategorie>().HasIndex(k => k.Name).IsUnique();
    }
}
