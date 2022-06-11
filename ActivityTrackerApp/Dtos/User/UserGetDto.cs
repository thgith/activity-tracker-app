namespace ActivityTrackerApp.Dtos
{
    public class UserGetDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public DateTime? DateJoined { get; set; }

        public DateTime? DateDeleted { get; set; }
    }
}