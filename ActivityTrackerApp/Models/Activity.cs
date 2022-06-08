using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActivityTrackerApp.Models
{
    /// <summary>
    /// An activity.
    /// </summary>
    [Table(nameof(Activity))]
    public class Activity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ActivityId { get; set; }

        /// <summary>
        /// Name of the activity.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Description of the activity.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The date the activity was started.
        /// </summary>
        [Required]
        public DateTime StartDateUtc { get; set; }

        /// <summary>
        /// The deadline the user set for the activity.
        /// </summary>
        public DateTime DueDateUtc { get; set; }

        /// <summary>
        /// The date the user manual set the activity as complete.
        /// </summary>
        public DateTime CompleteDateUtc { get; set; }

        /// <summary>
        /// The date the activity was archived.
        /// The user can still see it in archived activities.
        /// </summary>
        public DateTime ArchiveDateUtc { get; set; }

        /// <summary>
        /// The date the activity was soft-deleted in the system.
        /// The user can no longer see it.
        /// </summary>
        public DateTime DeletedDateUtc { get; set; }

        /// <summary>
        /// The custom color of the activity for rendering.
        /// </summary>
        [MaxLength(7)]
        public string ColorHex { get; set; }

        /// <summary>
        /// A list of tags associated with the activity.
        /// Used for sorting and filtering.
        /// </summary>
        [MaxLength(10)]
        public IList<string> Tags { get; set; }
    }
}
