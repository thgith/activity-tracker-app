using Microsoft.AspNetCore.Mvc;

namespace ActivityTrackerApp.Controllers
{
    /// <summary>
    /// Base controller that other controllers inherit from.
    /// </summary>
    [Produces("application/json")]
    [ApiController]
    public class ApiControllerBase : Controller
    {
        // TODO set up logger here

        public ApiControllerBase()
        {
            // TODO set up logger here
        }

    }
}
