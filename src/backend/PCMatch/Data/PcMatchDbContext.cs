using Microsoft.EntityFrameworkCore;
using PCMatch.Models;

namespace PCMatch.Data;

public class PcMatchDbContext : DbContext
{
    public PcMatchDbContext(DbContextOptions<PcMatchDbContext> options) : base(options) { }

    public DbSet<Component> Components { get; set; }
    public DbSet<Cpu> Cpus { get; set; }
    public DbSet<Gpu> Gpus { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<FpsBenchmark> FpsBenchmarks { get; set; }
    public DbSet<Motherboard> Motherboards { get; set; }
    public DbSet<Ram> Rams { get; set; }
    public DbSet<Psu> Psus { get; set; }
    public DbSet<Storage> Storages { get; set; }
    public DbSet<Case> Cases { get; set; }
    public DbSet<Purpose> Purposes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cpu>().HasOne(c => c.Component).WithOne().HasForeignKey<Cpu>(c => c.Id);
        modelBuilder.Entity<Gpu>().HasOne(g => g.Component).WithOne().HasForeignKey<Gpu>(g => g.Id);
        modelBuilder.Entity<Motherboard>().HasOne(m => m.Component).WithOne().HasForeignKey<Motherboard>(m => m.Id);
        modelBuilder.Entity<Ram>().HasOne(r => r.Component).WithOne().HasForeignKey<Ram>(r => r.Id);
        modelBuilder.Entity<Psu>().HasOne(p => p.Component).WithOne().HasForeignKey<Psu>(p => p.Id);
        modelBuilder.Entity<Storage>().HasOne(s => s.Component).WithOne().HasForeignKey<Storage>(s => s.Id);
        modelBuilder.Entity<Case>().HasOne(c => c.Component).WithOne().HasForeignKey<Case>(c => c.Id);
    }
}