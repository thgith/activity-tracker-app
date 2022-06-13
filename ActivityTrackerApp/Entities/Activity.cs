using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    [MaxLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// Description of the activity.
    /// </summary>
    [MaxLength(1000)]
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
    public bool? IsArchived { get; set; }

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
    /// <remarks>No more than 10 tags allowed.</remarks>
    [MaxLength(10)]
    public IList<string> Tags { get; set; }

    public DateTime? DateDeletedUtc { get; set; }

    [Required]
    public Guid OwnerId { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; }

    /// <summary>
    /// The sessions associated with the Activity
    /// </summary>
    [NotMapped]
    public IList<Session> Sessions { get; set; }

}
