using ActivityTrackerApp.Constants;
using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Services;
using Microsoft.AspNetCore.Mvc;
namespace ActivityTrackerApp.Controllers;


/// <summary>
/// Auth endpoints. 
/// Endpoint will be: api/v1/Auth
/// </summary>
[Route("api/v1/[controller]")]
public class AuthController : ApiControllerBase<AuthController>
{
    private readonly IHelperService _helperService;

    /// <summary>
    /// Constructor.
    /// </summary>
    public AuthController(
        IHelperService helperService,
        IUserService userService,
        IJwtService jwtService,
        ILogger<AuthController> logger) : base(userService, jwtService, logger)
    {
        _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="userRegisterDto">The registration data.</param>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> RegisterAsync([FromBody] UserRegisterDto userRegisterDto)
    {
        try
        {
            // Note: model annotations handle a lot of basic error checking
            if (!_helperService.IsEmailValid(userRegisterDto.Email))
            {
                return Problem("Invalid Email", statusCode: StatusCodes.Status400BadRequest);
            }

            if (await userService.IsEmailTaken(userRegisterDto.Email))
            {
                return Problem("Email already taken", statusCode: StatusCodes.Status400BadRequest);
            }

            var userRegisterDtoWithToken = await userService.RegisterUserAsync(userRegisterDto);

            // Add token to user's cookies
            Response.Cookies.Append(
                GlobalConstants.JWT_TOKEN_COOKIE_NAME,
                userRegisterDtoWithToken.Token,
                new CookieOptions
                {
                    HttpOnly = true,
                    IsEssential = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Domain = "localhost",
                    Expires = DateTime.UtcNow.AddHours(GlobalConstants.AUTH_COOKIE_EXPIRATION_HOURS)
                });

            Response.Cookies.Append(
                GlobalConstants.CURR_USER_ID_COOKIE_NAME,
                userRegisterDtoWithToken.Entity.Id.ToString(),
                new CookieOptions
                {
                    HttpOnly = false,
                    IsEssential = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Domain = "localhost",
                    Expires = DateTime.UtcNow.AddHours(GlobalConstants.AUTH_COOKIE_EXPIRATION_HOURS)
                });
            return Ok(userRegisterDtoWithToken);
        }
        catch (Exception e)
        {
            var message = "There was an error registering";
            logger.LogError(message, e.Message, e.StackTrace);
            return Problem(message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Logs in the user by creating and setting auth cookies.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> LoginAsync([FromBody] UserLoginDto userLoginDto)
    {
        try
        {
            // Note: model annotations handle a lot of error checking for requirements
            if (!_helperService.IsEmailValid(userLoginDto.Email))
            {
                Problem("Invalid Email", statusCode: StatusCodes.Status400BadRequest);
            }

            if (await userService.IsEmailTaken(userLoginDto.Email))
            {
                Problem("Email already taken", statusCode: StatusCodes.Status400BadRequest);
            }

            var useGetDtoWithToken = await userService.AuthenticateUserAsync(userLoginDto);

            if (useGetDtoWithToken == null)
            {
                return Problem("Incorrect username/password combination", statusCode: StatusCodes.Status400BadRequest);
            }

            // Add token to user's cookies
            Response.Cookies.Append(
                GlobalConstants.JWT_TOKEN_COOKIE_NAME,
                useGetDtoWithToken.Token,
                new CookieOptions
                {
                    HttpOnly = true,
                    IsEssential = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Domain = "localhost",
                    Expires = DateTime.UtcNow.AddHours(GlobalConstants.AUTH_COOKIE_EXPIRATION_HOURS)

                });

            Response.Cookies.Append(GlobalConstants.CURR_USER_ID_COOKIE_NAME,
                useGetDtoWithToken.Entity.Id.ToString(),
                new CookieOptions
                {
                    HttpOnly = false,
                    IsEssential = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Domain = "localhost",
                    Expires = DateTime.UtcNow.AddHours(GlobalConstants.AUTH_COOKIE_EXPIRATION_HOURS)

                });
            return Ok(useGetDtoWithToken);
        }
        catch (Exception e)
        {
            var message = "There was an error logging in";
            logger.LogError(message, e.Message, e.StackTrace);
            return Problem(message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Logs out the user by clearing the auth cookies.
    /// </summary>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult Logout()
    {
        try
        {
            // Remove token from user's cookies
            Response.Cookies.Delete(
                GlobalConstants.JWT_TOKEN_COOKIE_NAME,
                // These options are so the cookies clear in Chrome
                new CookieOptions()
                {
                    SameSite = SameSiteMode.None,
                    Secure = true
                });
            Response.Cookies.Delete(
                GlobalConstants.CURR_USER_ID_COOKIE_NAME,
                // These options are so the cookies clear in Chrome
                new CookieOptions()
                {
                    SameSite = SameSiteMode.None,
                    Secure = true
                });
            return Ok("Successfully logged out");
        }
        catch (Exception e)
        {
            var message = "There was an error logging out";
            logger.LogError(message, e.Message, e.StackTrace);
            return Problem(message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}