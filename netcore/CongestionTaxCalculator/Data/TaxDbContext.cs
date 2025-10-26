using Congestion.Calculator.Models;

namespace Congestion.Calculator.Data;

using Microsoft.EntityFrameworkCore;

public class TaxDbContext(DbContextOptions<TaxDbContext> options) : DbContext(options)
{
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Passage> Passages => Set<Passage>();
    public DbSet<TaxRule> TaxRules => Set<TaxRule>();
    public DbSet<Holiday> Holidays => Set<Holiday>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Vehicle>().HasKey(v => v.PlateNumber);
        modelBuilder.Entity<Passage>().HasKey(p => p.Id);
        modelBuilder.Entity<TaxRule>().HasKey(t => new { t.Start, t.End });
        modelBuilder.Entity<Holiday>().HasKey(h => h.Date);
    }
}