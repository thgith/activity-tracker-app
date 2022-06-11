namespace ActivityTrackerApp.Services
{
    public interface IHelperService
    {
        /// <summary>
        /// Determines whether the email is valid.
        /// </summary>
        /// <param name="emailAddress">The email to check.</param>
        /// <returns><c>true</c> if the email is valid, <c>false</c> otherwise</returns>
        bool IsEmailValid(string emailAddress);
    }
}