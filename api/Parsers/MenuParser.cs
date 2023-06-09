using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Xml;
using TheMostAmazingLunchAPI.Models;
using TheMostAmazingLunchAPI.Utils;

namespace TheMostAmazingLunchAPI.Parsers;

public abstract class MenuParser
{
    protected static readonly HttpClient _httpClient = new();

    protected readonly Restaurant _restaurant;

    protected WeekMenu _weekMenu;

    protected MenuParser(Restaurant restaurant)
    {
        _restaurant = restaurant;
        _weekMenu = GetBlankWeekMenu();
    }

    private WeekMenu GetBlankWeekMenu()
    {
        var dayMenus = new List<DayMenu>(DateUtil.DaysInWeek);
        for (int dayIndex = 0; dayIndex < DateUtil.DaysInWeek; dayIndex++)
        {
            dayMenus.Add(new DayMenu()
            {
                RestaurantId = _restaurant.Id,
                Date = DateUtil.GetDateFromCurrentWeekDay(dayIndex),
                MenuItems = new List<MenuItem>()
            });
        }

        return new WeekMenu()
        {
            RestaurantId = _restaurant.Id,
            Year = DateUtil.GetCurrentYear(),
            WeekNr = DateUtil.GetCurrentWeekNr(),
            DayMenus = dayMenus
        };
    }

    /**
     * Helper methods for child classes to use for parsing
     */

    private async Task<string?> GetContentsFromUri(string uri)
    {
        try
        {
            using HttpResponseMessage response = await _httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
        catch
        {
            return null;
        }
    }

    protected async Task<HtmlNode?> GetHtmlFromSource()
    {
        string? htmlContents = await GetContentsFromUri(_restaurant.Source!);
        if (htmlContents != null)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContents);
            return htmlDoc.DocumentNode;
        }
        return null;
    }

    protected async Task<XmlElement?> GetXmlFromSource()
    {
        string? xmlContents = await GetContentsFromUri(_restaurant.Source!);
        if (xmlContents != null)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlContents);
            return xmlDoc.DocumentElement;
        }
        return null;
    }

    protected async Task<JObject?> GetJsonFromSource()
    {
        string? jsonContents = await GetContentsFromUri(_restaurant.Source!);
        if (jsonContents != null)
        {
            return JObject.Parse(jsonContents);
        }
        return null;
    }

    private static readonly string[] InvalidMenuItemTexts =
    {
        "stängt", "klämdag", "nationaldag"
    };

    protected bool IsValidMenuItem(string menuItemText)
    {
        return !string.IsNullOrWhiteSpace(menuItemText) &&
            !InvalidMenuItemTexts.Any(invalidText => menuItemText.ToLower().Contains(invalidText));
    }

    /**
     * Methods for individual parsing
     */

    public abstract Task<WeekMenu> GetCurrentWeekMenu();

    public static MenuParser GetParser(Restaurant restaurant)
    {
        switch (restaurant.Name)
        {
            case "The Factory":
            case "Ericofood":
                return new EricssonParser(restaurant);
            case "Tele2 Foodandco":
                return new FoodandcoParser(restaurant);
            case "Tastory":
                return new TastoryParser(restaurant);
            case "Upper East":
            case "Provence":
                return new KvartersMenynParser(restaurant);
            case "Eatery Kista Gate":
            case "Eatery Kista Nod":
                return new EateryParser(restaurant);
            case "Isas Spis":
                return new IsasSpisParser(restaurant);
            case "Sabis Nordic Forum":
                return new SabisNordicForumParser(restaurant);
            case "Heat":
                return new HeatParser(restaurant);
            case "Kista Garden":
                return new KistaGardenParser(restaurant);
            default:
                throw new NotImplementedException();
        }
    }
}
