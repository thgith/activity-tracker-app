using System.ComponentModel.DataAnnotations;
using ActivityTrackerApp.Constants;
using Microsoft.EntityFrameworkCore;

namespace ActivityTrackerApp.Entities;

/// <summary>
/// User info.
/// </summary>
[Index(nameof(Email))]
public class User : BaseEntity
{
    /// <summary>
    /// User's first name.
    /// </summary>
    [Required]
    [MinLength(1), MaxLength(50)]
    public string FirstName { get; set; }
    
    /// <summary>
    /// User's last name.
    /// </summary>
    [Required]
    [MinLength(1), MaxLength(50)]
    public string LastName { get; set; }
    
    /// <summary>
    /// User's email address.
    /// </summary>
    [Required]
    [MinLength(6), MaxLength(200)]
    public string Email { get; set; }
    
    /// <summary>
    /// User's password after hashing.
    /// </summary>
    [Required]
    [MinLength(8), MaxLength(256)]
    public string PasswordHash { get; set; }
    
    /// <summary>
    /// The date the user joined.
    /// </summary>
    [Required]
    public DateTime JoinDateUtc { get; set; }

    /// <summary>
    /// The date the user was deleted. <c>null</c> if active user.
    /// </summary>
    public DateTime? DeletedDateUtc { get; set; }

    /// <summary>
    /// The role of the user. <see cref="Roles"></see>
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Role { get; set; }
}