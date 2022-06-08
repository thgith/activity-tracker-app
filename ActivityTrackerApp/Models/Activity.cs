namespace ActivityTrackerApp.Models
{
    ///<summary>
    /// An activity.
    ///</summary>
    public class Activity : BaseEntity
    {
        ///<summary>
        /// Name of the activity.
        ///</summary>
        public string Title { get; set; }

        ///<summary>
        /// Description of the activity.
        ///</summary>
        public string Description { get; set; }

        ///<summary>
        /// The date the activity was started.
        ///</summary>
        public DateTime StartDateUtc { get; set; }

        ///<summary>
        /// The deadline the user set for the activity.
        ///</summary>
        public DateTime DueDateUtc { get; set; }

        ///<summary>
        /// The date the user manual set the activity as complete.
        ///</summary>
        public DateTime CompleteDateUtc { get; set; }

        ///<summary>
        /// The date the activity was archived.
        /// The user can still see it in archived activities.
        ///</summary>
        public DateTime ArchiveDateUtc { get; set; }

        ///<summary>
        /// The date the activity was soft-deleted in the system.
        /// The user can no longer see it.
        ///</summary>
        public DateTime DeletedDateUtc { get; set; }

        ///<summary>
        /// The custom color of the activity for rendering.
        ///</summary>
        public string Color { get; set; }

        ///<summary>
        /// A list of tags associated with the activity.
        /// Used for sorting and filtering.
        ///</summary>
        public string[] Tags { get; set; }
    }
}
