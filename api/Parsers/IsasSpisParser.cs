using HtmlAgilityPack;
using TheMostAmazingLunchAPI.Models;
using TheMostAmazingLunchAPI.Utils;

namespace TheMostAmazingLunchAPI.Parsers;

public class IsasSpisParser : MenuParser
{
    public IsasSpisParser(Restaurant restaurant) : base(restaurant)
    {
    }

    public override async Task<WeekMenu> GetCurrentWeekMenu()
    {
        HtmlNode? htmlBody = await GetHtmlFromSource();
        if (htmlBody != null)
        {
            var dayHtmlContainer = htmlBody.SelectSingleNode(".//div[@id='current']")?.SelectSingleNode(".//div[@class='menu-col']");
            if (dayHtmlContainer != null)
            {
                var dayHtmlNodes = dayHtmlContainer.SelectNodes(".//div[contains(@class, 'menu-item')]");
                foreach (var dayHtmlNode in dayHtmlNodes)
                {
                    var dayIndex = DateUtil.GetWeekDayIndexFromSwedishText(dayHtmlNode.SelectSingleNode(".//h6").InnerText);
                    if (dayIndex != null)
                    {
                        var menuItems = new List<MenuItem>();
                        var htmlMenuItems = dayHtmlNode.SelectNodes(".//p[not(@class)]");
                        foreach (var htmlMenuItem in htmlMenuItems ?? new HtmlNodeCollection(null))
                        {
                            var menuItemText = htmlMenuItem.InnerText.Trim();
                            if (IsValidMenuItem(menuItemText))
                            {
                                menuItems.Add(new MenuItem()
                                {
                                    Contents = menuItemText
                                });
                            }
                        }
                        _weekMenu.DayMenus!.ElementAt(dayIndex.Value).MenuItems = menuItems;
                    }
                }
            }
        }
        return _weekMenu;
    }
}
