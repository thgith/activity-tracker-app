using ActivityTrackerApp.Constants;

namespace ActivityTrackerApp.Dtos;

/// <summary>
/// User. For GETs and data returned.
/// </summary>
public class UserGetDto
{
    /// <summary>
    /// Identifier of the entity.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User's first name.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// User's last name.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// User's email.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// The date the user joined.
    /// </summary>
    public DateTime? JoinDateUtc { get; set; }
    
    /// <summary>
    /// The role of the user. <see cref="Roles"></see>
    /// </summary>
    public string Role { get; set; }
}