using HtmlAgilityPack;
using Microsoft.AspNetCore.Html;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using TheMostAmazingLunchAPI.Models;
using TheMostAmazingLunchAPI.Utils;

namespace TheMostAmazingLunchAPI.Parsers;

public class EateryParser : MenuParser
{
    private static readonly Dictionary<string, string> RestaurantPaths = new Dictionary<string, string>
    {
        { "Eatery Kista Gate", "/kista-gate" },
        { "Eatery Kista Nod", "/kista-nod" }
    };

    public EateryParser(Restaurant restaurant) : base(restaurant)
    {
    }

    public override async Task<WeekMenu> GetCurrentWeekMenu()
    {
        JObject? jsonObject = await GetJsonFromSource();
        if (jsonObject != null)
        {
            var menuIdObject = jsonObject["eateries"]?[RestaurantPaths[_restaurant.Name!]]?["menues"];
            if (menuIdObject != null && menuIdObject.Type != JTokenType.Array)
            {
                var menuId = menuIdObject?.Value<string>("lunchmeny");
                if (menuId != null)
                {
                    var menusContent = jsonObject["menues"]?[menuId]?["content"]?.Value<string>("content");
                    if (menusContent != null)
                    {
                        var htmlBody = new HtmlDocument();
                        htmlBody.LoadHtml(menusContent);

                        var dayHtmlNodes = htmlBody.DocumentNode.SelectNodes("//p");
                        for (int dayIndex = 0; dayIndex < dayHtmlNodes?.Count; dayIndex++)
                        {
                            var dayHtmlNode = dayHtmlNodes[dayIndex];
                            var potentialDayTitle = dayHtmlNode.SelectSingleNode(".//b");
                            if (potentialDayTitle != null)
                            {
                                List<MenuItem> menuItems = new();
                                var currentDayIndex = DateUtil.GetWeekDayIndexFromSwedishText(potentialDayTitle.InnerText);
                                if (currentDayIndex != null)
                                {
                                    string[] lines = dayHtmlNode.InnerText.Split("\n");
                                    for (int lineIndex = 1; lineIndex < lines.Length; lineIndex++)
                                    {
                                        if (IsValidMenuItem(lines[lineIndex]))
                                        {
                                            menuItems.Add(new MenuItem()
                                            {
                                                Contents = lines[lineIndex]
                                            });
                                        }                                        
                                    }
                                    _weekMenu.DayMenus!.ElementAt(currentDayIndex.Value).MenuItems = menuItems;
                                }
                            }
                        }
                    }
                }
            }            
        }
        return _weekMenu;
    }
}
