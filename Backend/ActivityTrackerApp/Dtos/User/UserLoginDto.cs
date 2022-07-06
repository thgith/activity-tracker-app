using System.ComponentModel.DataAnnotations;

namespace ActivityTrackerApp.Dtos;

public class UserLoginDto
{
    [Required]
    [MinLength(6), MaxLength(200)]
    public string Email { get; set; }

    [Required]
    [MinLength(8), MaxLength(50)]
    public string Password { get; set; }
}