namespace ActivityTrackerApp.Exceptions;

/// <summary>
/// Exception that indicates the requester cannot perform an action
/// because they are not authenticated.
/// </summary>
[Serializable]
public class UnauthenticatedException : Exception
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public UnauthenticatedException()
        : base("The current user is not properly authenticated")
    {

    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">The readable message to include with the error.</param>
    public UnauthenticatedException(string message)
        : base(message)
    {

    }
}