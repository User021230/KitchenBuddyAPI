using Microsoft.EntityFrameworkCore;
using KitchenBuddyAPI.Models;

namespace KitchenBuddyAPI.Data;

public class KitchenBuddyDbContext : DbContext
{
    public KitchenBuddyDbContext(DbContextOptions<KitchenBuddyDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Recipe> Recipes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Unique constraints
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Username).IsUnique();

            // Required fields
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Surname).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Usertype).IsRequired().HasMaxLength(20);

            // Default values
            entity.Property(e => e.IsEmailVerified).HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        modelBuilder.Entity<Recipe>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Ingridients).IsRequired();
                entity.Property(e => e.Directions).IsRequired();
                entity.Property(e => e.NutritionalBenefits).IsRequired();

                 entity.HasOne(e => e.User)
                        .WithMany(u => u.Recipes)
                        .HasForeignKey(e => e.UserId)
                        .OnDelete(DeleteBehavior.Cascade); 
            }
        );
    }
}