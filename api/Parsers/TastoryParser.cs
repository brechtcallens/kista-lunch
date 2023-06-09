using HtmlAgilityPack;
using System.Web;
using System.Xml;
using TheMostAmazingLunchAPI.Models;

namespace TheMostAmazingLunchAPI.Parsers;

public class TastoryParser : MenuParser
{
    public TastoryParser(Restaurant restaurant) : base(restaurant)
    {
    }

    public override async Task<WeekMenu> GetCurrentWeekMenu()
    {
        XmlElement? xmlBody = await GetXmlFromSource();
        if (xmlBody != null)
        {
            var dayXmlNodes = xmlBody.SelectNodes(".//item");
            for (int dayIndex = 0; dayIndex < dayXmlNodes?.Count; dayIndex++)
            {
                // Extract all menu items for the specified day.
                var menuItems = new List<MenuItem>();
                try
                {
                    HtmlDocument htmlSnippet = new HtmlDocument();
                    var htmlContents = dayXmlNodes[dayIndex].SelectSingleNode(".//description").InnerText;
                    htmlSnippet.LoadHtml(htmlContents);

                    var pTags = htmlSnippet.DocumentNode.SelectNodes(".//p");
                    for (int pTagIndex = 0; pTagIndex < pTags?.Count; pTagIndex++)
                    {
                        var menuItemText = HttpUtility.HtmlDecode(pTags[pTagIndex].InnerText).Trim();
                        if (IsValidMenuItem(menuItemText) &&
                            !menuItemText.Contains("Till dagens lunch har vi alltid") &&
                            !menuItemText.Contains("Med reservation"))
                        {
                            menuItems.Add(new MenuItem()
                            {
                                Contents = menuItemText
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
