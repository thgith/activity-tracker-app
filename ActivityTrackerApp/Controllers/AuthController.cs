using ActivityTrackerApp.Constants;
using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ActivityTrackerApp.Controllers
{
    /// <summary>
    /// An activity. 
    /// Endpoint will be: api/v1/Auth
    /// </summary>
    [Route("api/v1/[controller]")]
    public class AuthController : ApiControllerBase
    {
        private readonly IUserService _userService;
        private readonly IHelperService _helperService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IUserService userService,
            IHelperService helperService,
            IMapper mapper,
            ILogger<AuthController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RegisterAsync([FromBody] UserRegisterDto userRegisterDto)
        {
            try
            {
                // Note: model annotation handle error checking for requirements
                if (!_helperService.IsEmailValid(userRegisterDto.Email))
                {
                    return BadRequest("Invalid Email");
                }

                if (await _userService.IsEmailTaken(userRegisterDto.Email))
                {
                    return BadRequest("Email already taken");
                }

                var userRegisterDtoWithToken = await _userService.RegisterUserAsync(userRegisterDto);
                
                // Add token to user's cookies
                Response.Cookies.Append(GlobalConstants.JWT_TOKEN_COOKIE_NAME, userRegisterDtoWithToken.Token, new CookieOptions
                {
                    HttpOnly = true
                });

                return Ok("Successfully registered!");
            }
            catch (Exception e)
            {
                var message = $"There was an error logging in: {e.Message}";
                _logger.LogError(message, e.StackTrace);
                return Problem(message, statusCode: 500);
            }
        }

        /// <inheritdoc/>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> LoginAsync([FromBody] UserUpdateDto userPutDto)
        {
            try
            {
                // Note: model annotation handle error checking for requirements
                if (!_helperService.IsEmailValid(userPutDto.Email))
                {
                    BadRequest("Invalid Email");
                }

                if (await _userService.IsEmailTaken(userPutDto.Email))
                {
                    BadRequest("Email already taken");
                }

                var userPutDtoWithToken = await _userService.AuthenticateUserAsync(userPutDto);

                if (userPutDtoWithToken == null)
                {
                    return BadRequest("Incorrect username/password combination");
                }

                // Add token to user's cookies
                Response.Cookies.Append(GlobalConstants.JWT_TOKEN_COOKIE_NAME, userPutDtoWithToken.Token, new CookieOptions
                {
                    HttpOnly = true
                });

                return Ok("Successfully logged in!");
            }
            catch (Exception e)
            {
                var message = $"There was an error logging in: {e.Message}";
                _logger.LogError(message, e.StackTrace);
                return Problem(message, statusCode: 500);
            }
        }

        [HttpPost("logout")]
        [AllowAnonymous]
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
                var message = $"There was an error logging out: {e.Message}";
                _logger.LogError(message, e.StackTrace);
                return Problem(message, statusCode: 500);
            }
        }
    }
}