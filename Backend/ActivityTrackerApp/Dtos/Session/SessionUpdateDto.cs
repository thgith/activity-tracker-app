using System.ComponentModel.DataAnnotations;

using ActivityTrackerApp.Constants;

namespace ActivityTrackerApp.Dtos;

/// <summary>
/// A timed session. Format from a PUT request.
/// </summary>
public class SessionUpdateDto
{
    /// <summary>
    /// The date the session was started.
    /// </summary>
    [Required]
    public DateTime? StartDateUtc { get; set; }

    /// <summary>
    /// How long the session is in seconds.
    /// </summary>
    [Required]
    public uint? DurationSeconds { get; set; }

    /// <summary>
    /// Optional additional notes.
    /// </summary>
    [MaxLength(GlobalConstants.LONG_TEXT_MAX_CHAR)]
    public string? Notes { get; set; }
}