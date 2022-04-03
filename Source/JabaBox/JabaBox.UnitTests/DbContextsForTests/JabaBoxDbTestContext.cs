using JabaBox.Core.Domain.Entities;
using JabaBox.Core.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace JabaBox.UnitTests.DbContextsForTests;

public sealed class JabaBoxDbTestContext : DbContext
{
    public JabaBoxDbTestContext()
    {
        Database.EnsureDeleted();
        Database.EnsureCreated();
    }
    
    public DbSet<AccountInfo> AccountInfos { get; set; }
    public DbSet<BaseDirectory> BaseDirectories { get; set; }
    public DbSet<StorageDirectory> StorageDirectories { get; set; }
    public DbSet<StorageFile> StorageFiles { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StorageFile>().Property(d => d.State)
            .HasConversion(new EnumToStringConverter<FileState>());

        modelBuilder.Entity<BaseDirectory>().HasKey(d => d.UserId);

        modelBuilder.Entity<BaseDirectory>().HasMany(d => d.Directories)
            .WithOne(s => s.BaseDirectory);

        modelBuilder.Entity<StorageDirectory>().HasMany(d => d.Files)
            .WithOne(f => f.Directory);

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("Test");
    }
}