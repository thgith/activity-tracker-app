using ActivityTrackerApp.Entities;

namespace ActivityTrackerApp.Dtos;

/// <summary>
/// An activity.
/// </summary>
public class ActivityGetDto : BaseEntity
{
    /// <summary>
    /// The ID of the user who owns the activity.
    /// </summary>
    public Guid OwnerId { get; set; }

    /// <summary>
    /// Name of the activity.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Description of the activity.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// The date the activity was started.
    /// </summary>
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
    /// The custom color of the activity for rendering.
    /// </summary>
    public string ColorHex { get; set; }

    /// <summary>
    /// A list of tags associated with the activity.
    /// Used for sorting and filtering.
    /// </summary>
    public IList<string>? Tags { get; set; }

    public virtual IList<SessionGetDto> Sessions { get; set; }
}