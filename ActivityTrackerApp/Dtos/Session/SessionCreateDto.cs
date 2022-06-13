using System.ComponentModel.DataAnnotations;

namespace ActivityTrackerApp.Dtos;

/// <summary>
/// A timed session.
/// </summary>
public class SessionCreateDto
{
    /// <summary>
    /// The ID of the activity the session is associated with.
    /// </summary>
    [Required]
    public Guid ActivityId { get; set; }

    [Required]
    public DateTime? StartDateUtc { get; set; }

    [Required]
    public uint DurationSeconds { get; set; } = 0;

    [MaxLength(1024)]
    public string Description { get; set; }
}