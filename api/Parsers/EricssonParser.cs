using HtmlAgilityPack;
using System.Text.RegularExpressions;
using TheMostAmazingLunchAPI.Models;

namespace TheMostAmazingLunchAPI.Parsers;

public class EricssonParser : MenuParser
{
    public EricssonParser(Restaurant restaurant) : base(restaurant)
    {
    }

    public override async Task<WeekMenu> GetCurrentWeekMenu()
    {
        HtmlNode? htmlBody = await GetHtmlFromSource();
        if (htmlBody != null)
        {
            var dayHtmlNodes = htmlBody.SelectNodes("//div[@class='day']");
            for (int dayIndex = 0; dayIndex < dayHtmlNodes?.Count; dayIndex++)
            {
                var dayHtmlNode = dayHtmlNodes[dayIndex];

                // Extract all menu items for the specified day.
                var menuItems = new List<MenuItem>();
                try
                {
                    var htmlMenuItemRows = dayHtmlNode.SelectNodes(".//div[@class='row']");
                    foreach (var htmlMenuItemRow in htmlMenuItemRows ?? new HtmlNodeCollection(null))
                    {
                        var menuItemContentsText = htmlMenuItemRow
                            .SelectSingleNode(".//div[contains(@class, 'title')]")
                            .InnerText
                            .Trim();
                        var menuItemPriceText = htmlMenuItemRow
                            .SelectSingleNode(".//div[contains(@class, 'price')]")
                            .InnerText;
                        var menuItemPrice = int.Parse(Regex.Match(menuItemPriceText, @"\d+").Value);
                        var menuItemAllergens = htmlMenuItemRow
                            .SelectSingleNode(".//div[contains(@class, 'allergens')]")
                            .SelectNodes(".//div");

                        if (IsValidMenuItem(menuItemContentsText))
                        {
                            menuItems.Add(new MenuItem()
                            {
                                Contents = menuItemContentsText,
                                Price = menuItemPrice
                            });
                        }                        
                    }
                }
                catch
                {
                    menuItems.Clear();
                }

                // Add extracted menu items as day menu to week menu.
                _weekMenu.DayMenus!.ElementAt(dayIndex).MenuItems = menuItems;
            }
        }

        return _weekMenu;
    }
}
