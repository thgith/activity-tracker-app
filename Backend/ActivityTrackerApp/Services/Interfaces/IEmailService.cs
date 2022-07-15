using ActivityTrackerApp.Entities;

namespace ActivityTrackerApp.Services
{
    /// <summary>
    /// Service for sending emails.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends the email after the user registers.
        /// </summary>
        public void SendWelcomeEmail(User user);

        /// <summary>
        /// Sends an email.
        /// </summary>
        public void SendEmail(User user, string subject, string body);
    }
}