using Microsoft.EntityFrameworkCore;
using System;

namespace NSDGenerator.Server.DbData;

public class NsdContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Diagram> Diagrams { get; set; }
    public DbSet<Block> Blocks { get; set; }
    public DbSet<RegistrationCode> RegistrationCodes { get; set; }

    public NsdContext(DbContextOptions<NsdContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("nsd");

        modelBuilder.Entity<Block>()
            .Property(e => e.BlockType)
            .HasConversion(v => v.ToString(), v => (EnumBlockType)Enum.Parse(typeof(EnumBlockType), v));

        modelBuilder.Entity<RegistrationCode>().HasData(InitRegistrationCodes());

        base.OnModelCreating(modelBuilder);
    }

    private static RegistrationCode[] InitRegistrationCodes()
    {
        return new RegistrationCode[]
        {
            new RegistrationCode{ Code="test01", IsActive=false },
        };
    }
}
