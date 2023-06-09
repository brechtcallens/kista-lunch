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
public class RestaurantController : ControllerBase
{
    RestaurantService _restaurantService;
    MenuService _menuService;

    public RestaurantController(RestaurantService restaurantService, MenuService menuService)
    {
        _restaurantService = restaurantService;
        _menuService = menuService;
    }

    /// <summary>
    /// Retrieves a list of currently supported restaurants in Kista.
    /// </summary>
    /// <remarks>Returns a list of all the restaurants currently supported by the API.</remarks>
    /// <response code="200">Returns the list of restaurants</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<Restaurant>> GetAll()
    {
        return Ok(_restaurantService.GetAll());
    }

    /// <summary>
    /// Retrieves a single restaurant by its ID.
    /// </summary>
    /// <remarks>Returns the specified restaurant if it exists, or an HTTP 404 otherwise.</remarks>
    /// <response code="200">Returns the restaurant that matches the ID.</response>
    /// <response code="404">If no restaurant with the ID exists.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Restaurant> Get(int id)
    {
        var restaurant = _restaurantService.Get(id);
        if (restaurant == null)
            return NotFound();
        return Ok(restaurant);
    }

    /// <summary>
    /// Retrieves the day lunch menu of the specified restaurant.
    /// </summary>
    /// <remarks>Returns the week menu of the specified restaurant if the restaurant exists, or an HTTP 404 otherwise.</remarks>
    /// <response code="200">Returns the week menu of the restaurant that matches the ID.</response>
    /// <response code="404">If no restaurant with the ID exists.</response>
    [HttpGet("{id}/menu")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WeekMenu>> GetMenu(int id)
    {
        var restaurant = _restaurantService.Get(id);
        if (restaurant == null)
            return NotFound();
        var weekMenu = await _menuService.GetCurrentWeekMenu(restaurant);
        return Ok(weekMenu);
    }
}
