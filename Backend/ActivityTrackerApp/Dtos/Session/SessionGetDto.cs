namespace ActivityTrackerApp.Dtos;

/// <summary>
/// A timed session. Format from a GET and what we return to the client.
/// </summary>
public class SessionGetDto
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// The ID of the activity the session is associated with.
    /// </summary>
    public Guid ActivityId { get; set; }

    /// <summary>
    /// The date the session was started.
    /// </summary>
    public DateTime StartDateUtc { get; set; }

    /// <summary>
    /// How long the session is in seconds.
    /// </summary>
    public uint DurationSeconds { get; set; }

    /// <summary>
    /// Optional additional notes.
    /// </summary>
    public string Notes { get; set; }
}