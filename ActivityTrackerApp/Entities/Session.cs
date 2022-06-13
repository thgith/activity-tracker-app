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
        [Required]
        public Guid ActivityId { get; set; }

        /// <remarks>
        /// We put this annotation so EF can associate Activity with ActivityId FK.
        /// Marked as virtual so that EF can lazy load.
        /// </remarks>
        [ForeignKey("ActivityId")]
        public virtual Activity Activity { get; set; }

        [Required]
        public DateTime? StartDateUtc { get; set; }

        [Required]
        [Column(TypeName="int")]
        public uint DurationSeconds { get; set; }

        [MaxLength(1024)]
        public string Description { get; set; }

        public DateTime? DeletedDateUtc { get; set; }
    }
}