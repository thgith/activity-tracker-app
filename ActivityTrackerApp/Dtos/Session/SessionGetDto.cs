using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActivityTrackerApp.Dtos
{
    /// <summary>
    /// A timed session.
    /// </summary>
    public class SessionGetDto
    {
        /// <summary>
        /// The ID of the activity the session is associated with.
        /// </summary>
        [Required]
        public Guid ActivityId { get; set; }

        [Required]
        public DateTime StartDateUtc { get; set; }

        [Required]
        public DateTime? EndDateUtc { get; set; }

        [MaxLength(1024)]
        public string Description { get; set; }
    }
}