using Microsoft.AspNetCore.Mvc;
using TheMostAmazingLunchAPI.Data;
using TheMostAmazingLunchAPI.Models;
using TheMostAmazingLunchAPI.Parsers;
using TheMostAmazingLunchAPI.Services;

namespace TheMostAmazingLunchAPI.Controllers;

[ApiController]
[Route("[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
public class MenuController : ControllerBase
{
    MenuService _menuService;

    public MenuController(MenuService menuService)
    {
        _menuService = menuService;
    }

    /// <summary>
    /// Returns the lunch menu of the day for all supported restaurants.
    /// </summary>
    /// <remarks>Returns day menu data for all restaurants currently supported. When no data is available, an empty list is returned.</remarks>
    /// <response code="200">Returns a list of day menus, or an empty list of no data was parsed.</response>
    [HttpGet("today")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IEnumerable<DayMenu?>> GetCurrentDay()
    {
        return await _menuService.GetAllCurrentDayMenus();
    }

    /// <summary>
    /// Returns the daily lunch menus for each day of the week, for all supported restaurants.
    /// </summary>
    /// <remarks>Returns week menu data for all restaurants currently supported. When no data is available, an empty list is returned.</remarks>
    /// <response code="200">Returns a list of week menus, or an empty list of no data was parsed.</response>
    [HttpGet("week")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IEnumerable<WeekMenu?>> GetCurrentWeek()
    {
        return await _menuService.GetAllCurrentWeekMenus();
    }

    /// <summary>
    /// Retrieves historical data of lunch menus of a certain year and week, for all supported restaurants.
    /// </summary>
    /// <param name="year">The year of the data. (e.g. 43)</param>
    /// <param name="week">The week number of the data. (e.g. 2023)</param>
    /// <remarks>Returns data from all restaurants supported at the specified date, which might not include all restaurants currently supported. When no data is available, an empty list is returned.</remarks>
    /// <response code="200">Returns a list of week menus for the specified year/week, or an empty list of no data was found.</response>
    [HttpGet("historical/{year}/{week}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IEnumerable<WeekMenu?>> GetHistoricalWeekMenu(int year, int week)
    {
        return await _menuService.GetAllWeekMenus(year, week);
    }
}
