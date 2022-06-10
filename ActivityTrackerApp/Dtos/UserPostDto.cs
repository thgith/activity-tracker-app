using ActivityTrackerApp.Entities;

namespace ActivityTrackerApp.Dtos
{
    public class UserPostDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public DateTime DateJoined { get; set; }
    }
}