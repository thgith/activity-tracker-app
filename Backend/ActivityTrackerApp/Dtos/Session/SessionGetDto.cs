namespace ActivityTrackerApp.Dtos;

/// <summary>
/// A timed session.
/// </summary>
public class SessionGetDto
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// The ID of the activity the session is associated with.
    /// </summary>
    public Guid ActivityId { get; set; }

    public DateTime StartDateUtc { get; set; }

    public uint DurationSeconds { get; set; }

    public string Notes { get; set; }
}