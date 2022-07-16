namespace ActivityTrackerAppTests.Helpers;

public static class TestHelpers
{
    /// <summary>
    /// Checks that the given dates are equal within the number of seconds.
    /// </summary>
    public static bool DatesEqualWithinSeconds(DateTime date, DateTime laterDate, int seconds = 60)
    {
        TimeSpan timeSpan = laterDate.Subtract(date);
        return timeSpan.TotalMinutes < seconds;
    }
}