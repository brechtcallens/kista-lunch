using HtmlAgilityPack;
using TheMostAmazingLunchAPI.Models;
using TheMostAmazingLunchAPI.Utils;

namespace TheMostAmazingLunchAPI.Parsers;

public class SabisNordicForumParser : MenuParser
{
    public SabisNordicForumParser(Restaurant restaurant) : base(restaurant)
    {
    }

    public override async Task<WeekMenu> GetCurrentWeekMenu()
    {
        HtmlNode? htmlBody = await GetHtmlFromSource();
        if (htmlBody != null)
        {
            var htmlDays = htmlBody.SelectNodes(".//ul[@class='menu-block__days']/li");
            foreach (var htmlDay in htmlDays ?? new HtmlNodeCollection(null))
            {
                var dayIndex = DateUtil.GetWeekDayIndexFromSwedishText(htmlDay.SelectSingleNode(".//h3").InnerText);
                if (dayIndex != null)
                {
                    var menuItems = new List<MenuItem>();
                    var htmlMenuItems = htmlDay.SelectNodes(".//p[@class='menu-block__dish-description']");
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
        return _weekMenu;
    }
}
