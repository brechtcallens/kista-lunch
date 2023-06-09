using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TheMostAmazingLunchAPI.Models;

[PrimaryKey(nameof(RestaurantId), nameof(Year), nameof(WeekNr))]
public class WeekMenu
{
    public int RestaurantId { get; set; }
    public int Year { get; set; }
    public int WeekNr { get; set; }
    public bool Success => DayMenus?.Count(dayMenu => dayMenu.Success) > 0;
    public IEnumerable<DayMenu>? DayMenus { get; set; }
}
