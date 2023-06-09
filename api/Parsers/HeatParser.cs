using System.Xml;
using TheMostAmazingLunchAPI.Models;
using TheMostAmazingLunchAPI.Utils;

namespace TheMostAmazingLunchAPI.Parsers;

public class HeatParser : MenuParser
{
    public HeatParser(Restaurant restaurant) : base(restaurant)
    {
    }

    public override async Task<WeekMenu> GetCurrentWeekMenu()
    {
        XmlElement? xmlBody = await GetXmlFromSource();
        if (xmlBody != null)
        {
            int previousDayIndex = -1;
            string currentMenuItem = "";
            List<MenuItem>? menuItems = new();

            var currentElement = xmlBody.SelectSingleNode(".//rubrik")?.NextSibling;
            while (currentElement != null)
            {
                if (currentElement.Name.Contains("ratt"))
                {
                    var elementNameSplit = currentElement.Name.Split("ratt"); // mandagratt1rubrik => [mandag, 1rubrik]
                    var dayIndex = DateUtil.GetWeekDayIndexFromSwedishText(elementNameSplit[0]);
                    if (dayIndex != null)
                    {
                        // New menu item starts
                        if (elementNameSplit[1].Contains("rubrik"))
                        {
                            // New day starts
                            if (previousDayIndex != dayIndex)
                            {
                                // Check if we were building a menu before and save it
                                if (previousDayIndex != -1)
                                {
                                    _weekMenu.DayMenus!.ElementAt(previousDayIndex).MenuItems = menuItems;
                                }                                
                                // Reset to new day
                                previousDayIndex = dayIndex.Value;
                                menuItems = new();
                            }

                            // Add start of menu item
                            currentMenuItem = currentElement.InnerText.Trim();
                        }
                        // Menu item continues
                        else if (elementNameSplit[1].Contains("text"))
                        {
                            currentMenuItem = (currentMenuItem + currentElement.InnerText).Trim();
                            if (IsValidMenuItem(currentMenuItem))
                            {
                                menuItems.Add(new MenuItem()
                                {
                                    Contents = currentMenuItem
                                });
                                currentMenuItem = "";
                            }
                        }
                    }                    
                }
                currentElement = currentElement.NextSibling;
            }

            // Save the last menu
            if (0 <= previousDayIndex && previousDayIndex < DateUtil.DaysInWeek)
            {
                _weekMenu.DayMenus!.ElementAt(previousDayIndex).MenuItems = menuItems;
            }
        }
        return _weekMenu;
    }
}
