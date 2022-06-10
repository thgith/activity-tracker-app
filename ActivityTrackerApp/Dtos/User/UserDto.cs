using ActivityTrackerApp.Entities;

namespace ActivityTrackerApp.Dtos
{
    public class UserDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public DateTime? DateJoined { get; set; }

        public DateTime? DateDeleted { get; set; }
    }
}