using System.ComponentModel.DataAnnotations;
using ActivityTrackerApp.Constants;

namespace ActivityTrackerApp.Dtos;

/// <summary>
/// User object for registering.
/// </summary>
public class UserRegisterDto
{
    /// <summary>
    /// User's first name.
    /// </summary>
    [Required]
    [MinLength(1), MaxLength(GlobalConstants.SHORT_TEXT_MAX_CHAR)]
    public string FirstName { get; set; }

    /// <summary>
    /// User's last name.
    /// </summary>
    [Required]
    [MinLength(1), MaxLength(GlobalConstants.SHORT_TEXT_MAX_CHAR)]
    public string LastName { get; set; }

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