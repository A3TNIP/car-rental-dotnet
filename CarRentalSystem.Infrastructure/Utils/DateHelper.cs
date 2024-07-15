namespace CarRentalSystem.Infrastructure.Utils;

public class DateHelper
{
    public static List<DateTime> GetDatesInRange(DateTime startDate, DateTime endDate)
    {
        var dates = new List<DateTime>();
        for (var dt = startDate; dt <= endDate; dt = dt.AddDays(1))
        {
            dates.Add(dt);
        }

        return dates;
    }
}