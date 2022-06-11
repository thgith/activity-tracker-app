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
    }
}
