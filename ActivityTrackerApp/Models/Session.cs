namespace ActivityTrackerApp.Models
{
    /// <summary>
    /// A timed session. 
    /// </summary>
    public class Session : BaseEntity
    {
        /// <summary>
        /// The ID of the activity the session is associated with.
        /// </summary>
        public Guid ActivityId { get; set; }

        public DateTime StartDateUtc { get; set; }

        public DateTime EndDateUtc { get; set; }
        
        public string Description { get; set; }
    }
}
