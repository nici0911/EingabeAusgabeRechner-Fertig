using Kassabuch.Models;
using Microsoft.EntityFrameworkCore;

namespace Kassabuch.Data;

public class KassabuchDbContext(DbContextOptions<KassabuchDbContext> options) : DbContext(options)
{
    public DbSet<Kassenbuchung> Kassenbuchungen => Set<Kassenbuchung>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Kassenbuchung>().HasIndex(b => b.Belegnummer).IsUnique();
        modelBuilder.Entity<Kassenbuchung>().Property(b => b.Einnahme).HasPrecision(12, 2);
        modelBuilder.Entity<Kassenbuchung>().Property(b => b.Ausgabe).HasPrecision(12, 2);
    }
}
