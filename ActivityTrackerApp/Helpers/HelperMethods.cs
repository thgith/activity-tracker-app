using System.Net.Mail;

namespace ActivityTrackerApp.Services
{
    /// <summary>
    /// Helper methods class.
    /// </summary>
    public class HelperMethods : IHelperMethods
    {
        public HelperMethods()
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
}