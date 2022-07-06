namespace ActivityTrackerApp.Dtos;

public class UserGetDto
{
    public Guid Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public DateTime? JoinDateUtc { get; set; }
    
    public string Role { get; set; }
}