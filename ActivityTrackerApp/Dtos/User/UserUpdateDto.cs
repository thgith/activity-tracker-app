using System.ComponentModel.DataAnnotations;

namespace ActivityTrackerApp.Dtos;

public class UserUpdateDto
{
    [MinLength(1), MaxLength(50)]
    public string? FirstName { get; set; }

    [MinLength(1), MaxLength(50)]
    public string? LastName { get; set; }

    [MinLength(6), MaxLength(200)]
    public string? Email { get; set; }

    [MinLength(8), MaxLength(50)]
    public string? Password { get; set; }
}