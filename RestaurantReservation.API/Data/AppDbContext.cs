using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RestaurantReservation.API.Models;

namespace RestaurantReservation.API.Data;

public class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<Table> Tables { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<MenuItem> MenuItems { get; set; }
    public DbSet<Reservation> Reservations { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Restaurant>()
            .HasMany(r => r.Categories)
            .WithMany(c => c.Restaurants);
    }
}