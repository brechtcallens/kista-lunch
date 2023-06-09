using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TheMostAmazingLunchAPI.Data;
using TheMostAmazingLunchAPI.Models;
using TheMostAmazingLunchAPI.Parsers;
using TheMostAmazingLunchAPI.Utils;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TheMostAmazingLunchAPI.Services;

public class MenuService
{
    private readonly LunchContext _lunchContext;

    public MenuService(LunchContext lunchContext) => _lunchContext = lunchContext;

    public async Task<WeekMenu?> GetWeekMenu(Restaurant restaurant, int year, int weekNr, bool saveChanges=true)
    {
        var existingWeekMenu = _lunchContext.WeekMenus
            .AsNoTracking()
            .Include(weekMenu => weekMenu.DayMenus)
            .FirstOrDefault(weekMenu => 
                weekMenu.RestaurantId == restaurant.Id 
                && weekMenu.Year == year 
                && weekMenu.WeekNr == weekNr);

        if (existingWeekMenu != null)
        {
            return existingWeekMenu;
        }
        else if (year == DateUtil.GetCurrentYear() && weekNr == DateUtil.GetCurrentWeekNr())
        {
            var newWeekMenu = await MenuParser.GetParser(restaurant).GetCurrentWeekMenu();
            if (newWeekMenu != null && newWeekMenu.Success && saveChanges)
            {
                _lunchContext.WeekMenus.Add(newWeekMenu);
                _lunchContext.SaveChanges();
            }               
            return newWeekMenu;
        }
        else
        {
            return null;
        }
    }

    public async Task<WeekMenu?> GetCurrentWeekMenu(Restaurant restaurant)
    {
        var year = DateUtil.GetCurrentYear();
        var weekNr = DateUtil.GetCurrentWeekNr();
        return await GetWeekMenu(restaurant, year, weekNr);
    }

    public async Task<IEnumerable<WeekMenu?>> GetAllWeekMenus(int year, int weekNr)
    {
        // Get all the restaurants for which to get the weekmenu.
        var restaurants = _lunchContext.Restaurants
            .AsNoTracking()
            .ToList();

        // Create parallel tasks for fetching the weekmenu and wait for them.
        var weekMenuTasks = restaurants
            .Select(restaurant => GetWeekMenu(restaurant, year, weekNr, false))
            .ToList();
        await Task.WhenAll(weekMenuTasks);
        
        // Get all weekmenus from the tasks' results.
        var weekMenus = weekMenuTasks
            .Select(task => task.Result)
            .Where(weekMenu => weekMenu != null)
            .ToList();

        // Save all new menus syncrhonously.
        foreach (var weekMenu in weekMenus)
        {
            if (weekMenu != null && weekMenu.Success && !_lunchContext.WeekMenus.Contains(weekMenu))
            {
                _lunchContext.WeekMenus.Add(weekMenu);
            }
        }
        _lunchContext.SaveChanges();
        
        return weekMenus;
    }

    public async Task<IEnumerable<WeekMenu?>> GetAllCurrentWeekMenus()
    {
        var year = DateUtil.GetCurrentYear();
        var weekNr = DateUtil.GetCurrentWeekNr();
        return await GetAllWeekMenus(year, weekNr);
    }

    public async Task<IEnumerable<DayMenu?>> GetAllCurrentDayMenus()
    {
        var weekMenus = await GetAllCurrentWeekMenus();
        
        List<DayMenu?> dayMenus = new();
        foreach (var weekMenu in weekMenus)
        {
            dayMenus.Add(weekMenu?.DayMenus?.FirstOrDefault(dayMenu => dayMenu.Date.Equals(DateOnly.FromDateTime(DateTime.Now))));
        }
        return dayMenus;
    }
}
