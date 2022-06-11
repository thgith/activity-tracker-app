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
    public class ActivityController : ApiControllerBase<ActivityController>, IActivityController
    {        
        private readonly IActivityService _activityService;

        public ActivityController(
            IUserService userService,
            IJwtService jwtService,
            IActivityService activityService,
            ILogger<ActivityController> logger) : base(userService, jwtService, logger)
        {
            _activityService = activityService ?? throw new ArgumentNullException(nameof(activityService));
        }

        /// <inheritdoc/>
        public Task<IActionResult> DeleteAsync(Guid activityId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<ActionResult<IEnumerable<ActivityDto>>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<ActionResult<ActivityDto>> GetAsync(Guid activityId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<ActionResult> PostAsync(ActivityPostDto activityPostDto)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IActionResult> PutAsync(Guid activityId, ActivityPutDto activityPutDto)
        {
            throw new NotImplementedException();
        }
    }
}
