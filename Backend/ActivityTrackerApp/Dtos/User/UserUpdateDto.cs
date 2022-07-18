using System.ComponentModel.DataAnnotations;
using ActivityTrackerApp.Constants;

namespace ActivityTrackerApp.Dtos;

/// <summary>
/// User object for updating user.
/// </summary>
public class UserUpdateDto
{
    /// <summary>
    /// User's first name.
    /// </summary>
    [MinLength(1), MaxLength(GlobalConstants.SHORT_TEXT_MAX_CHAR)]
    public string? FirstName { get; set; }

    /// <summary>
    /// User's last name.
    /// </summary>
    [MinLength(1), MaxLength(GlobalConstants.SHORT_TEXT_MAX_CHAR)]
    public string? LastName { get; set; }

    /// <summary>
    /// User's email.
    /// </summary>
    [MinLength(6), MaxLength(200)]
    public string? Email { get; set; }

    /// <summary>
    /// User's password.
    /// </summary>
    [MinLength(8), MaxLength(GlobalConstants.SHORT_TEXT_MAX_CHAR)]
    public string? Password { get; set; }
}