using System.Security.Claims;
using ActivityTrackerApp.Constants;
using ActivityTrackerApp.Exceptions;
using ActivityTrackerApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ActivityTrackerApp.Controllers;

/// <summary>
/// Base controller that other controllers inherit from.
/// </summary>
[Produces("application/json")]
[ApiController]
public abstract class ApiControllerBase<T> : Controller
{
    /// <summary>
    /// Service for working with user data.
    /// </summary>
    protected readonly IUserService userService;

    /// <summary>
    /// Service for working with JWT data.
    /// </summary>
    protected readonly IJwtService jwtService;

    /// <summary>
    /// Logger.
    /// </summary>
    protected readonly ILogger<T> logger;

    /// <summary>
    /// Constructor.
    /// </summary>
    public ApiControllerBase(
        IUserService userService,
        IJwtService jwtService,
        ILogger<T> logger)
    {
        this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        this.jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    /// <summary>
    /// Checks if the user is currently authenticated and then carry out
    /// the <c>actionMethod</c> if they are. Else throw an err.
    /// </summary>
    /// <param name="request">The request object.</param>
    /// <param name="actionMethod">The method to run if the user is authenticated.</param>
    protected async Task<IActionResult> checkAuthAndPerformAction(
        HttpRequest request,
        Func<Guid, Task<IActionResult>> actionMethod)
    {
        try
        {
            // -- Verify the user is authenticated --
            var jwtCookie = Request.Cookies[GlobalConstants.JWT_TOKEN_COOKIE_NAME];
            var token = jwtService.CheckAuthenticated(jwtCookie);

            // -- Get current user from token --
            // We verified token above, so this data should be correct
            token.Payload.TryGetValue(ClaimTypes.NameIdentifier, out var currUserId);
            var currUserIdStr = currUserId as string;
            Guid.TryParse(currUserIdStr, out var currUserGuid);

            // -- Try to update --
            return await actionMethod(currUserGuid);
        }
        catch (UnauthenticatedException e)
        {
            logger.LogError(e.Message);
            return Problem(e.Message, statusCode: StatusCodes.Status401Unauthorized);
        }
        catch (ForbiddenException e)
        {
            logger.LogError(e.Message);
            return Problem(e.Message, statusCode: StatusCodes.Status403Forbidden);
        }
        catch (InvalidDataException e)
        {
            logger.LogError(e.Message);
            return Problem(e.Message, statusCode: StatusCodes.Status400BadRequest);
        }
        catch (Exception e)
        {
            var message = "There was a problem performing the action";
            logger.LogError(message, e.Message, e.StackTrace);
            return Problem(message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}