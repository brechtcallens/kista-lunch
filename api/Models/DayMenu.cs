using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TheMostAmazingLunchAPI.Utils;

namespace TheMostAmazingLunchAPI.Models;

[PrimaryKey(nameof(RestaurantId), nameof(Date))]
public class DayMenu
{
    public int RestaurantId { get; set; }
    public DateOnly Date { get; set; }
    public bool Success => MenuItems?.Count() > 0;
    public IEnumerable<MenuItem>? MenuItems { get; set; }

}
