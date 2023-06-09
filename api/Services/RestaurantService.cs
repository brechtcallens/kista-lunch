using Microsoft.EntityFrameworkCore;
using TheMostAmazingLunchAPI.Data;
using TheMostAmazingLunchAPI.Models;

namespace TheMostAmazingLunchAPI.Services;

public class RestaurantService
{
    private readonly LunchContext _context;

    public RestaurantService(LunchContext context) => _context = context;

    public Restaurant? Get(int id)
    {
        return _context.Restaurants
            .AsNoTracking()
            .FirstOrDefault(restaurant => restaurant.Id == id);
    }

    public IEnumerable<Restaurant> GetAll()
    {
        return _context.Restaurants
            .AsNoTracking()
            .ToList();
    }
}
