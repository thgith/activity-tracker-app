using ActivityTrackerApp.Services;

namespace ActivityTrackerApp.Controllers
{
    /// <summary>
    /// Activity endpoints.
    /// </summary>
    public class ActivityController : ApiControllerBase
    {        
        IActivityService _activityService;

        public ActivityController(IActivityService activityService)
        {
            _activityService = activityService ?? throw new ArgumentNullException(nameof(activityService));
        }

        // GET
        // CREATE
        // UPDATE
        // DELETE
    }
}
