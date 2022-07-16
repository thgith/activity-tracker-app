namespace ActivityTrackerAppTests.Helpers;

/// <summary>
/// Test helpers.
/// </summary>
public static class TestHelpers
{
    /// <summary>
    /// Checks that the given dates are equal within the number of seconds.
    /// </summary>
    /// <param name="date">The first date.</param>
    /// <param name="laterDate">The second date.</param>
    /// <param name="confidenceIntervalSeconds">The number of seconds in between the dates allowed.</param>
    public static bool DatesEqualWithinSeconds(DateTime date, DateTime laterDate, int confidenceIntervalSeconds = 30)
    {
        TimeSpan timeSpan = laterDate.Subtract(date);
        return timeSpan.TotalMinutes < confidenceIntervalSeconds;
    }
}