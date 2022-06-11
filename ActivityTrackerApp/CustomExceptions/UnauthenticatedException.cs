namespace ActivityTrackerApp.Exceptions
{
    [Serializable]
    class UnauthenticatedException : Exception
    {
        public UnauthenticatedException() {  }

        public UnauthenticatedException(string name)
            : base(String.Format("The current user is not properly authenticated"))
        {

        }
    }
}