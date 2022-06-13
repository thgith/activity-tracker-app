namespace ActivityTrackerApp.Exceptions;

[Serializable]
class UnauthenticatedException : Exception
{
    public UnauthenticatedException()
        : base("The current user is not properly authenticated")
    {

    }

    public UnauthenticatedException(string message)
        : base(message)
    {

    }
}