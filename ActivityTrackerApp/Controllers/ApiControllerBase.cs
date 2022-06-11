using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ActivityTrackerApp.Constants;
using ActivityTrackerApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ActivityTrackerApp.Controllers
{
    /// <summary>
    /// Base controller that other controllers inherit from.
    /// </summary>
    [Produces("application/json")]
    [ApiController]
    public abstract class ApiControllerBase<T> : Controller
    {
        protected readonly IUserService userService;
        protected readonly IJwtService jwtService;
        protected readonly ILogger<T> logger;

        public ApiControllerBase(
            IUserService userService,
            IJwtService jwtService,
            ILogger<T> logger)
        {
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected async Task<ActionResult> checkAuthenticatedAndAuthorized(
            HttpRequest request,
            Guid? userIdOfResource,
            bool allowIfSameUser = true)
        {
            // Get JWT info from cookie. Should be stored from login/registration
            var jwtCookie = request.Cookies[GlobalConstants.JWT_TOKEN_COOKIE_NAME];
            if (jwtCookie == null)
            {
                return Unauthorized("You are not properly authenticated");
            }

            // Verify that the token is still valid
            JwtSecurityToken token;
            try
            {
                token = jwtService.Verify(jwtCookie);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Unauthorized("You are not properly authenticated");
            }

            // Check if user has permission
            token.Payload.TryGetValue(ClaimTypes.NameIdentifier, out var currentUserId);
            var currentUserIdStr = currentUserId as string;
            Guid.TryParse(currentUserIdStr, out var currentUserGuid);

            var isCurrUserAuthorized = await userService.IsCurrentUserAuthorized(currentUserGuid, userIdOfResource, allowIfSameUser);
            if (!isCurrUserAuthorized)
            {
                return Forbid("You are not authorized to access this");
            }
            
            return null;
        }
    }
}
