using System.ComponentModel.DataAnnotations;

namespace ActivityTrackerApp.Dtos;

/// <summary>
/// User object for changing passwords.
/// </summary>
public class UserChangePasswordDto
{
    /// <summary>
    /// User's email.
    /// </summary>
    [Required]
    [MinLength(6), MaxLength(200)]
    public string Email { get; set; }

    /// <summary>
    /// User's old password. Will be checked against current password.
    /// </summary>
    [Required]
    [MinLength(8), MaxLength(50)]
    public string OldPassword { get; set; }

    /// <summary>
    /// User's new password.
    /// </summary>
    [Required]
    [MinLength(8), MaxLength(50)]
    public string NewPassword { get; set; }
}