using System.ComponentModel.DataAnnotations;
using ActivityTrackerApp.Constants;

namespace ActivityTrackerApp.Dtos;

/// <summary>
/// A timed session. Format from a POST request.
/// </summary>
public class SessionCreateDto
{
    /// <summary>
    /// The ID of the activity the session is associated with.
    /// </summary>
    [Required]
    public Guid ActivityId { get; set; }

    /// <summary>
    /// The date the session was started.
    /// </summary>
    [Required]
    public DateTime? StartDateUtc { get; set; }

    /// <summary>
    /// How long the session is in seconds.
    /// </summary>
    [Required]
    public uint DurationSeconds { get; set; } = 0;

    /// <summary>
    /// Optional additional notes.
    /// </summary>
    [MaxLength(GlobalConstants.LONG_TEXT_MAX_CHAR)]
    public string Notes { get; set; }
}