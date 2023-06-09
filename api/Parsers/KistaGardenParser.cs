using HtmlAgilityPack;
using TheMostAmazingLunchAPI.Models;
using TheMostAmazingLunchAPI.Utils;

namespace TheMostAmazingLunchAPI.Parsers;

public class KistaGardenParser : MenuParser
{
    public KistaGardenParser(Restaurant restaurant) : base(restaurant)
    {
    }

    public override async Task<WeekMenu> GetCurrentWeekMenu()
    {
        HtmlNode? htmlBody = await GetHtmlFromSource();
        if (htmlBody != null)
        {
            var dayHtmlNodes = htmlBody.SelectSingleNode("//div[contains(@class, 'elementor-tabs-content-wrapper')]")
                .SelectNodes(".//div[contains(@class, 'elementor-tab-content')]");
            foreach (var dayHtmlNode in dayHtmlNodes ?? new HtmlNodeCollection(null))
            {
                var menuItems = new List<MenuItem>();
                var sectionTitles = dayHtmlNode.SelectNodes(".//h2");
                foreach (var sectionTitle in sectionTitles ?? new HtmlNodeCollection(null))
                {
                    // Extract the category for the menu items
                    var category = string.Empty;
                    if (sectionTitle.InnerText.ToLower().Contains("bröd"))
                    {
                        continue;
                    }
                    else if (sectionTitle.InnerText.ToLower().Contains("special"))
                    {
                        category = "SPECIAL: ";
                    }
                    else if (sectionTitle.InnerText.ToLower().Contains("dagens"))
                    {
                        category = "DAGENS: ";
                    }
                    else if (sectionTitle.InnerText.ToLower().Contains("vegetarisk"))
                    {
                        category = "VEG: ";
                    }

                    // Find the next container with menu items for category
                    var sibling = sectionTitle.NextSibling;
                    while (sibling != null && sibling.OriginalName != "div")
                    {
                        sibling = sibling.NextSibling;
                    }

                    // Find all items in the category
                    if (sibling != null)
                    {
                        var titles = sibling.SelectNodes(".//span[contains(@class, 'wcpt-title')]");
                        var excerpts = sibling.SelectNodes(".//div[contains(@class, 'wcpt-excerpt')]");
                        var amounts = sibling.SelectNodes(".//span[contains(@class, 'wcpt-amount')]");
                        for (int index = 0; index < titles?.Count(); index++)
                        {
                            menuItems.Add(new MenuItem()
                            {
                                Contents = (category + titles[index].InnerText + " - " + excerpts[index].InnerText).Replace("\n", " "),
                                Price = int.Parse(amounts[index].InnerText)
                            });
                        }
                    }                    
                }

                // Finally save the menu items
                var dayIndex = DateUtil.GetWeekDayIndexFromSwedishText(dayHtmlNode.SelectSingleNode(".//h2").InnerText);
                if (dayIndex != null)
                {
                    _weekMenu.DayMenus!.ElementAt(dayIndex.Value).MenuItems = menuItems;
                }                
            }
        }
        return _weekMenu;
    }
}
