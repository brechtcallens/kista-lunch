using Newtonsoft.Json;
using TheMostAmazingLunchAPI.Models;

namespace TheMostAmazingLunchAPI.Data;

public static class DbInitializer
{
    public static void initialize(LunchContext context)
    {
        List<Restaurant> restaurants = new();

        // Extract restaurants from JSON file.
        var serializer = new JsonSerializer();
        using (var fileReader = new StreamReader("restaurants.json"))
        using (var jsonTextReader = new JsonTextReader(fileReader))
        {
            restaurants = serializer.Deserialize<List<Restaurant>>(jsonTextReader) ?? new List<Restaurant>();
        }

        // Compare JSON restaurants with contents of database and add/update respectively.
        foreach (var restaurant in restaurants)
        {
            if (context.Restaurants.Any(dbRestaurant => dbRestaurant.Name == restaurant.Name))
            {
                context.Restaurants.Update(restaurant);
            }
            else
            {
                context.Restaurants.Add(restaurant);
            }
        }
        context.SaveChanges();
    }
}
