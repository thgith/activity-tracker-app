using System.ComponentModel.DataAnnotations;

namespace ActivityTrackerApp.Dtos;

/// <summary>
/// A timed session.
/// </summary>
public class SessionUpdateDto
{
    [Required]
    public DateTime? StartDateUtc { get; set; }

    [Required]
    public uint? DurationSeconds { get; set; }

    [MaxLength(1024)]
    public string? Notes { get; set; }
}