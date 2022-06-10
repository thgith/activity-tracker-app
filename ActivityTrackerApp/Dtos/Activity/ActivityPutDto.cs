using System.ComponentModel.DataAnnotations;

namespace ActivityTrackerApp.Dtos
{
    /// <summary>
    /// An activity.
    /// </summary>
    public class ActivityPutDto
    {
        /// <summary>
        /// Name of the activity.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string? Name { get; set; }

        /// <summary>
        /// Description of the activity.
        /// </summary>
        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// The deadline the user set for the activity.
        /// </summary>
        public DateTime? DueDateUtc { get; set; }

        /// <summary>
        /// The date the user manual set the activity as complete.
        /// </summary>
        public DateTime? CompleteDateUtc { get; set; }

        /// <summary>
        /// The date the activity was archived.
        /// The user can still see it in archived activities.
        /// </summary>
        public DateTime? ArchiveDateUtc { get; set; }

        /// <summary>
        /// The custom color of the activity for rendering.
        /// </summary>
        [MaxLength(7)]
        public string? ColorHex { get; set; }

        /// <summary>
        /// A list of tags associated with the activity.
        /// Used for sorting and filtering.
        /// </summary>
        [MaxLength(10)]
        public IList<string>? Tags { get; set; }
    }
}
