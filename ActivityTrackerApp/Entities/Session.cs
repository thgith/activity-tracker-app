using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActivityTrackerApp.Entities
{
    /// <summary>
    /// A timed session.
    /// </summary>
    public class Session : BaseEntity
    {
        /// <summary>
        /// The ID of the activity the session is associated with.
        /// </summary>
        /// <remarks>
        /// The annotation is using the name of the corresponding C# model,
        /// not the name of the table, which is pluralized.
        /// </remarks>
        [Required]
        public Guid ActivityId { get; set; }

        /// <remarks>
        /// We put this annotation so EF can associate Activity with ActivityId FK.
        /// Marked as virtual so that EF can lazy load.
        /// </remarks>
        [ForeignKey("ActivityId")]
        public virtual Activity Activity { get; set; }

        [Required]
        public DateTime StartDateUtc { get; set; }

        [Required]
        public DateTime? EndDateUtc { get; set; }

        [MaxLength(1024)]
        public string Description { get; set; }
    }
}