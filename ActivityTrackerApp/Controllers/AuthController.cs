using ActivityTrackerApp.Constants;
using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ActivityTrackerApp.Controllers
{
    /// <summary>
    /// Auth endpoints. 
    /// Endpoint will be: api/v1/Auth
    /// </summary>
    [Route("api/v1/[controller]")]
    public class AuthController : ApiControllerBase<AuthController>
    {
        private readonly IHelperService _helperService;

        public AuthController(
            IHelperService helperService,
            IUserService userService,
            IJwtService jwtService,
            ILogger<AuthController> logger) : base(userService, jwtService, logger)
        {
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
        }

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
                    return BadRequest("Invalid Email");
                }

                if (await userService.IsEmailTaken(userRegisterDto.Email))
                {
                    return BadRequest("Email already taken");
                }

                var userRegisterDtoWithToken = await userService.RegisterUserAsync(userRegisterDto);
                
                // Add token to user's cookies
                Response.Cookies.Append(GlobalConstants.JWT_TOKEN_COOKIE_NAME, userRegisterDtoWithToken.Token, new CookieOptions
                {
                    HttpOnly = true
                });

                return Ok("Successfully registered!");
            }
            catch (Exception e)
            {
                var message = "There was an error registering";
                logger.LogError(message, e.Message, e.StackTrace);
                return Problem(message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

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
                    BadRequest("Invalid Email");
                }

                if (await userService.IsEmailTaken(userLoginDto.Email))
                {
                    BadRequest("Email already taken");
                }

                var userLoginDtoWithToken = await userService.AuthenticateUserAsync(userLoginDto);

                if (userLoginDtoWithToken == null)
                {
                    return BadRequest("Incorrect username/password combination");
                }

                // Add token to user's cookies
                Response.Cookies.Append(GlobalConstants.JWT_TOKEN_COOKIE_NAME, userLoginDtoWithToken.Token, new CookieOptions
                {
                    HttpOnly = true
                });

                return Ok("Successfully logged in!");
            }
            catch (Exception e)
            {
                var message = "There was an error logging in";
                logger.LogError(message, e.Message, e.StackTrace);
                return Problem(message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult Logout()
        {
            try
            {
                // Remove token from user's cookies
                Response.Cookies.Delete(GlobalConstants.JWT_TOKEN_COOKIE_NAME);
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
}