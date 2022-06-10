using ActivityTrackerApp.Entities;

namespace ActivityTrackerApp.Services
{
    public interface IEmailService
    {
        public void SendWelcomeEmail(User user);
        public void SendEmail(User user, string subject, string body);
    }
}