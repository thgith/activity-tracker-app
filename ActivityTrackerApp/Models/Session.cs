using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActivityTrackerApp.Models
{
    /// <summary>
    /// A timed session. 
    /// </summary>
    public class Session
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SessionId { get; set; }

        /// <summary>
        /// The ID of the activity the session is associated with.
        /// EF should recognize this as a foreign key.
        /// </summary>
        [Required]
        public Guid ActivityId { get; set; }

        [Required]
        public DateTime StartDateUtc { get; set; }

        [Required]
        public DateTime EndDateUtc { get; set; }
        
        [MaxLength(1024)]
        public string Description { get; set; }
    }
}
