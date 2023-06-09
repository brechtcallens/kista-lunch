using HtmlAgilityPack;
using TheMostAmazingLunchAPI.Models;
using TheMostAmazingLunchAPI.Utils;

namespace TheMostAmazingLunchAPI.Parsers;
public class KvartersMenynParser : MenuParser
{
    public KvartersMenynParser(Restaurant restaurant) : base(restaurant)
    {
    }

    public override async Task<WeekMenu> GetCurrentWeekMenu()
    {
        HtmlNode? htmlBody = await GetHtmlFromSource();
        if (htmlBody != null)
        {
            var lunchHtmlNode = htmlBody.SelectSingleNode("//div[@class='meny']");
            if (lunchHtmlNode != null)
            {
                var lines = lunchHtmlNode.InnerHtml.Split("<br>");

                DayMenu? currentDayMenu = null;
                List<MenuItem> currentMenuItems = new();
                
                foreach (var line in lines)
                {
                    // New day menu starts
                    if (line.Contains("strong"))
                    {
                        // Check and finish if we were building a daymenu
                        if (currentDayMenu != null && currentMenuItems.Count > 0)
                        {
                            currentDayMenu.MenuItems = currentMenuItems;
                            currentMenuItems = new();
                        }

                        // Parse the current title as a weekday
                        int? currentDayIndex = DateUtil.GetWeekDayIndexFromSwedishText(line);
                        if (currentDayIndex != null)
                        {
                            currentDayMenu = _weekMenu.DayMenus!.ElementAt(currentDayIndex.Value);
                        }
                        else
                        {
                            currentDayMenu = null;
                        }                            
                    }
                    // Currently building a day menu
                    else if (currentDayMenu != null && IsValidMenuItem(line))
                    {
                        // New menu item starts
                        if (line.Contains(':'))
                        {
                            currentMenuItems.Add(new MenuItem()
                            {
                                Contents = line
                            });
                        }
                        // Stupid overflow text gets added to last item
                        else if (currentMenuItems.Count > 0)
                        {
                            currentMenuItems.Last().Contents += " " + line;
                        }
                    }
                }
            }
        }
        return _weekMenu;
    }
}
