namespace ActivityTrackerApp.Constants;

/// <summary>
/// Determines what a user has access to.
/// </summary>
public static class Roles
{
    /// <summary>
    /// A regular user.
    /// </summary>
    public const string MEMBER = "member";   

    /// <summary>
    /// Admin. Has permissions to update other users' data.
    /// </summary>
    public const string ADMIN = "admin";   
}