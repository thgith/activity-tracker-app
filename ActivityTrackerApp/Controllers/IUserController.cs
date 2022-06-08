namespace ActivityTrackerApp.Controllers
{
    /// <summary>
    /// User endpoints.
    /// </summary>
    public interface IUserController : BaseController
    {
        public User GetUser(Guid userId);
    }
}