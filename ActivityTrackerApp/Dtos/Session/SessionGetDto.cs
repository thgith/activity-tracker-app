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
        public Guid ActivityId { get; set; }

        public DateTime StartDateUtc { get; set; }

        public uint DurationSeconds { get; set; }

        public string Description { get; set; }
    }
}