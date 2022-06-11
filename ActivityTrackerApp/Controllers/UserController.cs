using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ActivityTrackerApp.Constants;
using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ActivityTrackerApp.Controllers
{
    /// <summary>
    /// An activity. 
    /// Endpoint will be: api/v1/User
    /// </summary>
    [Route("api/v1/[controller]")]
    public class UserController : ApiControllerBase, IUserController
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;
        private readonly IHelperService _helperService;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IUserService userService,
            IJwtService jwtService,
            IHelperService helperService,
            ILogger<UserController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<UserGetDto>>> GetAllUsersAsync()
        {
            try
            {
                // Check permissions
                var authErr = await _checkAuthenticatedAndAuthorized(Request, null);
                if (authErr != null)
                {
                    return authErr;
                }

                // Get all users
                var usersDtos = await _userService.GetAllUsersAsync();
                return Ok(usersDtos);
            }
            catch (Exception e)
            {
                var message = $"There was an error getting the users";
                _logger.LogError(message, e.Message, e.StackTrace);
                return Problem(message, statusCode: 500);
            }
        }

        /// <inheritdoc/>
        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserGetDto>> GetUserAsync(Guid userId)
        {
            try
            {
                // Check permissions
                var authErr = await _checkAuthenticatedAndAuthorized(Request, userId);
                if (authErr != null)
                {
                    return authErr;
                }

                // Get user
                var user = await _userService.GetUserAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            catch (Exception e)
            {
                var message = $"There was an error getting the user with ID {userId}";
                _logger.LogError(message, e.Message, e.StackTrace);
                return Problem(message);
            }
        }

        /// <inheritdoc/>
        [HttpPut("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserAsync(
            Guid userId,
            [FromBody] UserUpdateDto userPutDto)
        {
            try
            {
                // Check permissions
                var authErr = await _checkAuthenticatedAndAuthorized(
                                        Request,
                                        userId,
                                        allowIfSameUser: true);
                if (authErr != null)
                {
                    return authErr;
                }

                // Check email
                if (userPutDto.Email != null && !_helperService.IsEmailValid(userPutDto.Email))
                {
                    return BadRequest("Invalid Email");
                }

                // Update user
                var res = await _userService.UpdateUserAsync(userId, userPutDto);
                if (res == null)
                {
                    return NotFound($"The user with the ID {userId} does not exist");
                }
                return Ok(res);
            }
            catch (Exception e)
            {
                var message = "There was an error creating the user";
                _logger.LogError(message, e.Message, e.StackTrace);
                return Problem(message, statusCode: 500);
            }
        }

        /// <inheritdoc/>
        [HttpDelete("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUserAsync(Guid userId)
        {
            try
            {
                // Check permissions
                var authErr = await _checkAuthenticatedAndAuthorized(
                                        Request,
                                        userId,
                                        allowIfSameUser: true);
                if (authErr != null)
                {
                    return authErr;
                }

                // Delete user
                var result = await _userService.DeleteUserAsync(userId);
                if (result)
                {
                    return Ok($"Successfully deleted user {userId}");
                }
                return NotFound($"User with ID {userId} does not exist");
            }
            catch (Exception e)
            {
                var message = $"There was a problem deleting the user with ID {userId}";
                _logger.LogError(message, e.Message, e.StackTrace);
                return Problem(message, statusCode: 500);
            }
        }
        
        private async Task<ActionResult> _checkAuthenticatedAndAuthorized(
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
                token = _jwtService.Verify(jwtCookie);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Unauthorized("You are not properly authenticated");
            }

            // Check if user has permission
            token.Payload.TryGetValue(ClaimTypes.NameIdentifier, out var currentUserId);
            var currentUserIdStr = currentUserId as string;
            Guid.TryParse(currentUserIdStr, out var currentUserGuid);

            var isCurrUserAuthorized = await _userService.IsCurrentUserAuthorized(currentUserGuid, userIdOfResource, allowIfSameUser);
            if (!isCurrUserAuthorized)
            {
                return Forbid("You are not authorized to access this");
            }
            
            return null;
        }
    }
}
