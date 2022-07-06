using System.ComponentModel.DataAnnotations;

namespace ActivityTrackerApp.Dtos;

public class UserChangePasswordDto
{
    [Required]
    [MinLength(6), MaxLength(200)]
    public string Email { get; set; }

    [Required]
    [MinLength(8), MaxLength(50)]
    public string OldPassword { get; set; }

    [Required]
    [MinLength(8), MaxLength(50)]
    public string NewPassword { get; set; }
}