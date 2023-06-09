namespace TheMostAmazingLunchAPI.Utils;

public static class DateUtil
{
    /**
     * Stating the obvious here to combat magic numbers in code
     */

    public static readonly int DaysInWeek = 7;
    
    /**
     * Current week and year information
     */

    public static int GetCurrentWeekNr() => System.Globalization.ISOWeek.GetWeekOfYear(DateTime.Now);

    public static int GetCurrentYear() => DateTime.Now.Year;

    /**
     * Weekday index to actual dates
     */

    private static DateOnly GetMondayDate() => DateOnly.FromDateTime(
        DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday));

    public static DateOnly GetDateFromCurrentWeekDay(int dayIndex) => GetMondayDate().AddDays(dayIndex);

    /**
     * Swedish text to weekday index
     */

    private static readonly string[] SwedishWeekDays = new string[]
    {
        "måndag", "tisdag", "onsdag", "torsdag", "fredag", "lördag", "söndag"
    };

    private static readonly string[] SwedishWeekDaysWithoutAccents = new string[]
    {
        "mandag", "tisdag", "onsdag", "torsdag", "fredag", "lordag", "sondag"
    };

    public static int? GetWeekDayIndexFromSwedishText(string weekDayText)
    {
        for (int i = 0; i < SwedishWeekDays.Length; i++)
        {
            if (weekDayText.ToLower().Contains(SwedishWeekDays[i]) || weekDayText.ToLower().Contains(SwedishWeekDaysWithoutAccents[i]))
            {
                return i;
            }
        }
        return null;
    }
}
