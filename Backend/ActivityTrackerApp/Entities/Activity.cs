using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using ActivityTrackerApp.Constants;

namespace ActivityTrackerApp.Entities;

/// <summary>
/// An activity.
/// </summary>
public class Activity : BaseEntity
{
    /// <summary>
    /// Name of the activity.
    /// </summary>
    [Required]
    [MaxLength(GlobalConstants.SHORT_TEXT_MAX_CHAR)]
    public string Name { get; set; }

    /// <summary>
    /// Description of the activity.
    /// </summary>
    [MaxLength(GlobalConstants.LONG_TEXT_MAX_CHAR)]
    public string Description { get; set; }

    /// <summary>
    /// The date the activity was started.
    /// </summary>
    [Required]
    public DateTime? StartDateUtc { get; set; }

    /// <summary>
    /// The deadline the user set for the activity.
    /// </summary>
    public DateTime? DueDateUtc { get; set; }

    /// <summary>
    /// The date the user manual set the activity as complete.
    /// </summary>
    public DateTime? CompletedDateUtc { get; set; }

    /// <summary>
    /// Whether the activity is archived.
    /// The user can still see it in archived activities.
    /// </summary>
    public bool IsArchived { get; set; }

    /// <summary>
    /// The date the activity was soft-deleted in the system.
    /// The user can no longer see it.
    /// </summary>
    public DateTime? DeletedDateUtc { get; set; }

    /// <summary>
    /// The custom color of the activity for rendering.
    /// </summary>
    [MaxLength(7)]
    public string ColorHex { get; set; }

    /// <summary>
    /// A list of tags associated with the activity.
    /// Used for sorting and filtering.
    /// </summary>
    /// <remarks>No more than 10 tags allowed. TODO: Need to add this check</remarks>
    public IList<string>? Tags { get; set; }

    /// <summary>
    /// The ID of the user who owns the activity.
    /// </summary>
    [Required]
    public Guid OwnerId { get; set; }

    /// <summary>
    /// User of the activity. Placed here as virtual for EF to create a FK.
    /// </summary>
    [ForeignKey("UserId")]
    public virtual User User { get; set; }

    /// <summary>
    /// The sessions associated with the Activity
    /// </summary>
    [NotMapped]
    public IList<Session> Sessions { get; set; }

}
