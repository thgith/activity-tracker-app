using System.ComponentModel.DataAnnotations;

namespace ActivityTrackerApp.Dtos
{
    /// <summary>
    /// A timed session.
    /// </summary>
    public class SessionPutDto
    {
        [Required]
        public DateTime? StartDateUtc { get; set; }

        [Required]
        public DateTime? EndDateUtc { get; set; }

        [MaxLength(1024)]
        public string? Description { get; set; }
    }
}