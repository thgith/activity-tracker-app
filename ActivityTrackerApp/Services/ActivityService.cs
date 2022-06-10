namespace ActivityTrackerApp.Services
{
    /// <summary>
    /// Activity service.
    /// </summary>
    public class ActivityService : IActivityService
    {
        private readonly IDataContext _dbContext;

        public ActivityService(IDataContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
    }
}