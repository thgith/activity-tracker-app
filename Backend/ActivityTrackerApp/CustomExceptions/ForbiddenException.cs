namespace ActivityTrackerApp.Exceptions;

/// <summary>
/// Exception that indicates a resource was forbidden.
/// </summary>
[Serializable]
public class ForbiddenException : Exception
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public ForbiddenException()
        : base("The current user does not have permission to perform the action")
    {}

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">The readable message to include with the error.</param>
    public ForbiddenException(string message)
        : base(message)
    {}
}