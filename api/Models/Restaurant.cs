namespace TheMostAmazingLunchAPI.Models;

public class Restaurant
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Reaction { get; set; }
    public RestaurantLocation? Location { get; set; }
    public string? Source { get; set; }
}

public class RestaurantLocation
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}