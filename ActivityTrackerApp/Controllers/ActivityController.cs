using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ActivityTrackerApp.Controllers
{
    /// <summary>
    /// Activity endpoints.
    /// </summary>
    [Route("api/v1/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ActivityController : ApiControllerBase, IActivityController
    {        
        private readonly IActivityService _activityService;
        private readonly ILogger _logger;

        public ActivityController(
            IActivityService activityService,
            ILogger logger)
        {
            _activityService = activityService ?? throw new ArgumentNullException(nameof(activityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        [Authorize]
        public Task<IActionResult> DeleteAsync(Guid activityId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [Authorize]
        public Task<ActionResult<IEnumerable<ActivityDto>>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [Authorize]
        public Task<ActionResult<ActivityDto>> GetAsync(Guid activityId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [Authorize]
        public Task<ActionResult> PostAsync(ActivityPostDto activityPostDto)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        [Authorize]
        public Task<IActionResult> PutAsync(Guid activityId, ActivityPutDto activityPutDto)
        {
            throw new NotImplementedException();
        }
    }
}
