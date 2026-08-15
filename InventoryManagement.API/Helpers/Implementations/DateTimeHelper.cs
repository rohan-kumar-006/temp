namespace InventoryManagement.API.Helpers.Implementations
{
    public static class DateTimeHelper
    {
        public static (DateTime StartUtc, DateTime EndUtc) GetTodayUtcRange()
        {
            var indiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById(OperatingSystem.IsWindows() ? "India Standard Time"
                : "Asia/Kolkata");

            var todayIndia = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow, indiaTimeZone).Date;

            return (TimeZoneInfo.ConvertTimeToUtc(todayIndia, indiaTimeZone),
                    TimeZoneInfo.ConvertTimeToUtc(todayIndia.AddDays(1), indiaTimeZone)
                );
        }
        public static (DateTime StartUtc, DateTime EndUtc) GetUtcRangeForIndiaDate(DateTime date)
        {
            var indiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById(
                OperatingSystem.IsWindows()
                    ? "India Standard Time"
                    : "Asia/Kolkata"
            );

            var startOfDayIndia = date.Date;
            var startOfNextDayIndia = startOfDayIndia.AddDays(1);

            var startUtc = TimeZoneInfo.ConvertTimeToUtc(
                startOfDayIndia,
                indiaTimeZone
            );

            var endUtc = TimeZoneInfo.ConvertTimeToUtc(
                startOfNextDayIndia,
                indiaTimeZone
            );

            return (startUtc, endUtc);
        }
    }
}
