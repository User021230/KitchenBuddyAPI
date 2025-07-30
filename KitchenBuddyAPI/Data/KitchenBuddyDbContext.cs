using Microsoft.EntityFrameworkCore;
using KitchenBuddyAPI.Models;

namespace KitchenBuddyAPI.Data;

public class KitchenBuddyDbContext : DbContext
{
    public KitchenBuddyDbContext(DbContextOptions<KitchenBuddyDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
}