using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace NSDGenerator.Server.Data;

public class NsdContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Diagram> Diagrams { get; set; }
    public DbSet<Block> Blocks { get; set; }

    public NsdContext(DbContextOptions<NsdContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("nsd");

        modelBuilder.Entity<Block>()
            .Property(e => e.BlockType)
            .HasConversion(v => v.ToString(), v => (EnumBlockType)Enum.Parse(typeof(EnumBlockType), v));

        modelBuilder.Entity<User>().HasData(InitUserData());

        base.OnModelCreating(modelBuilder);
    }

    private static User[] InitUserData()
    {
        return new User[]
        {
                new User{Name="user@starzec.net", Password="123", IsEnabled=true, Created=DateTime.Now},
        };
    }
}
