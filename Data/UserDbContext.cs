using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SeminarIntegration.DTOs;
using SeminarIntegration.Models;

namespace SeminarIntegration.Data;

public class UserDbContext(IOptions<DbSettings> dbSettings) : DbContext
{
    private readonly DbSettings _dbsettings = dbSettings.Value;

    // DbSet property to represent the table in the database
    public DbSet<User> Users { get; set; }
    public DbSet<DeletedUserHistory> DeletedUserHistory { get; set; }

    // Configuring the database provider and connection string
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_dbsettings.ConnectionString);
    }

    // Configuring the model
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");

            // Set Uuid as the primary key
            entity.HasKey(u => u.Uuid);

            // Set unique constraints
            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();

            // Configure required fields
            entity.Property(u => u.Username).IsRequired();
            entity.Property(u => u.Email).IsRequired();
            entity.Property(u => u.Password).IsRequired();
        });

        modelBuilder.Entity<DeletedUserHistory>(
            entity =>
            {
                entity.ToTable("DeletedUserHistory");

                // Set Uuid as the primary key
                entity.HasKey(u => u.Id);

                // can have multiple records with same user details
                entity.HasIndex(u => u.Username);
            });
    }
}