using ActivityTrackerApp.Entities;

namespace ActivityTrackerApp.Services
{
    /// <summary>
    /// Service for sending emails.
    /// </summary>
    public interface IEmailService
    {
        public void SendWelcomeEmail(User user);

        public void SendEmail(User user, string subject, string body);
    }
}