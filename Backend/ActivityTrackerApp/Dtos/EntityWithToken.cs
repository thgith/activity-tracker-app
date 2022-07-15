namespace ActivityTrackerApp.Dtos;

/// <summary>
/// Represents an entity with a JWT token.
/// </summary>
public class EntityWithToken<T>
{
    /// <summary>
    /// The entity.
    /// </summary>
    public T Entity { get; set; }

    /// <summary>
    /// The JWT token.
    /// </summary>
    public string Token { get; set; }
}