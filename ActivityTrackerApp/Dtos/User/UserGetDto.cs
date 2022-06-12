namespace ActivityTrackerApp.Dtos
{
    public class UserGetDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public DateTime? DateJoinedUtc { get; set; }

        public DateTime? DateDeletedUtc { get; set; }
    }
}