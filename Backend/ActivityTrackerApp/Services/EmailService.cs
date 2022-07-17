using ActivityTrackerApp.Entities;

using MailKit.Net.Smtp;

using MimeKit;

namespace ActivityTrackerApp.Services;

/// <inheritdoc/>
public class EmailService : IEmailService
{
    private readonly ISmtpClient _smtpClient;
    private readonly ILogger _logger;

    /// <summary>
    /// Constructor.
    /// </summary>
    public EmailService(
        ISmtpClient smtpClient,
        ILogger logger)
    {
        _smtpClient = smtpClient ?? throw new ArgumentNullException(nameof(smtpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public void SendWelcomeEmail(User user)
    {
        // TODO update this later
        var subject = "Welcome to ActivityTrackerApp";
        var body = $"Hi, how are you {user.FirstName}";
        SendEmail(user, subject, body);
    }

    /// <inheritdoc/>
    public void SendEmail(User user, string subject, string body)
    {
        // TODO update this later
        var port = 587;
        // TODO this is gmail. Change later
        var host = "smtp.gmail.com";
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("ActivityTrackerAppTest", "activitytrackerapptest@test.com"));
        message.To.Add(new MailboxAddress($"{user.FirstName} {user.LastName}", user.Email));
        message.Subject = subject;
        message.Body = new TextPart("plain")
        {
            Text = body
        };
        _smtpClient.Connect(host, port, true); // TODO enable SSL?
        _smtpClient.Authenticate("username", "pw_goes_here");
        _smtpClient.Send(message);
        _smtpClient.Disconnect(true);
    }
}