using Microsoft.EntityFrameworkCore;
using TheMostAmazingLunchAPI.Models;

namespace TheMostAmazingLunchAPI.Data;
public class LunchContext : DbContext
{
    public LunchContext(DbContextOptions<LunchContext> options) : base(options)
    {
    }

    public DbSet<Restaurant> Restaurants => Set<Restaurant>();

    public DbSet<WeekMenu> WeekMenus => Set<WeekMenu>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Restaurant>()
            .OwnsOne(restaurant => restaurant.Location);

        modelBuilder.Entity<DayMenu>()
            .OwnsMany(dayMenu => dayMenu.MenuItems, menuItem => menuItem.HasKey("Id"));
    }
}
