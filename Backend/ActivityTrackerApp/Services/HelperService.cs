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