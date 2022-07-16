using System.Net.Mail;

namespace ActivityTrackerApp.Services;

/// <inheritdocs/>
public class HelperService : IHelperService
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public HelperService()
    {

    }

    /// <summary>
    /// Removes the ms portion from the date.
    /// TODO: use this later b/c I'm going to have to update tests
    /// </summary>
    /// <param name="date">The date to shorten.</param>
    public DateTime ShortenDateTimeUtc(DateTime date)
    {
        return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, DateTimeKind.Utc);
    }

    /// <inheritdocs/>
    public bool IsEmailValid(string emailAddress)
    {
        try
        {
            new MailAddress(emailAddress);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}