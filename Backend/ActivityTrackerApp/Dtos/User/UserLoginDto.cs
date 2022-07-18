using System.ComponentModel.DataAnnotations;
using ActivityTrackerApp.Constants;

namespace ActivityTrackerApp.Dtos;

/// <summary>
/// User object for logging in.
/// </summary>
public class UserLoginDto
{
    /// <summary>
    /// User's email.
    /// </summary>
    [Required]
    [MinLength(6), MaxLength(200)]
    public string Email { get; set; }

    /// <summary>
    /// User's password.
    /// </summary>
    [Required]
    [MinLength(8), MaxLength(GlobalConstants.SHORT_TEXT_MAX_CHAR)]
    public string Password { get; set; }
}