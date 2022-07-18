using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ActivityTrackerApp.Constants;

namespace ActivityTrackerApp.Entities;

/// <summary>
/// A timed session.
/// </summary>
public class Session : BaseEntity
{
    /// <summary>
    /// The ID of the activity the session is associated with.
    /// </summary>
    [Required]
    public Guid ActivityId { get; set; }

    /// <remarks>
    /// We put this annotation so EF can associate Activity with ActivityId FK.
    /// Marked as virtual so that EF can lazy load.
    /// </remarks>
    [ForeignKey("ActivityId")]
    public virtual Activity Activity { get; set; }

    /// <summary>
    /// The date the session was started.
    /// </summary>
    [Required]
    public DateTime? StartDateUtc { get; set; }

    /// <summary>
    /// How long the session is in seconds.
    /// </summary>
    [Required]
    [Column(TypeName="int")]
    public uint DurationSeconds { get; set; }

    /// <summary>
    /// Optional additional notes.
    /// </summary>
    [MaxLength(GlobalConstants.LONG_TEXT_MAX_CHAR)]
    public string Notes { get; set; }

    /// <summary>
    /// The date the session was soft-deleted.
    /// </summary>
    public DateTime? DeletedDateUtc { get; set; }
}