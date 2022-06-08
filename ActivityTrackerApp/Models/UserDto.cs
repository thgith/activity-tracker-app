namespace ActivityTrackerApp.Models
{
    /// <summary>
    /// User info (frontend).
    /// </summary>
    public class UserDto : BaseEntity
    {
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public string Email { get; set; }
        
        public string Password { get; set; }
        
        public DateTime DateJoined { get; set; }

        public bool DateDeleted { get; set; }
    }
}
