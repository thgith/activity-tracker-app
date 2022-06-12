namespace ActivityTrackerApp.Exceptions
{
    [Serializable]
    class ForbiddenException : Exception
    {
        public ForbiddenException()
            : base("The current user does not have permission to perform the action")
        {

        }

        public ForbiddenException(string message)
            : base(message)
        {

        }
    }
}