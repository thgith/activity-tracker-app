namespace ActivityTrackerApp.Exceptions
{
    [Serializable]
    class ForbiddenException : Exception
    {
        public ForbiddenException() {  }

        public ForbiddenException(string name)
            : base(String.Format("The current user does not have permission to perform the action"))
        {

        }
    }
}