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
        private readonly IHelperMethods _helperService;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IUserService userService,
            IJwtService jwtService,
            IHelperMethods helperService,
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
        public async Task<ActionResult<IEnumerable<UserGetDto>>> GetAllAsync()
        {
            try
            {
                // Get the JWT cookie
                var jwt = Request.Cookies[GlobalConstants.JWT_TOKEN_COOKIE_NAME];

                // The user has not logged in yet
                if (jwt == null)
                {
                    return Unauthorized("You are unauthorized to access this endpoint");
                }

                // Verify that the token is still valid
                var token = _jwtService.Verify(jwt);

                // Make sure the current user has access to the requested user
                token.Payload.TryGetValue(ClaimTypes.NameIdentifier, out var storedUserId);
                var currentUserIdStr = storedUserId as string;

                // If the user is neither an admin or getting their info
                Guid.TryParse(currentUserIdStr, out var currentUserGuid);

                var isAdmin = await _userService.IsAdmin(currentUserGuid);
                if (!isAdmin)
                {
                    return Unauthorized("The current user is not authorized to access this endpoint");
                }

                var usersDtos = await _userService.GetAllUsersAsync();
                return Ok(usersDtos);
            }
            catch (Exception e)
            {
                var message = $"There was an error getting the users: {e.Message}";
                _logger.LogError(message, e.StackTrace);
                return Problem(message, statusCode: 500);
            }
        }

        /// <inheritdoc/>
        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserGetDto>> GetAsync(Guid userId)
        {
            try
            {
                // Get the JWT cookie
                var jwt = Request.Cookies[GlobalConstants.JWT_TOKEN_COOKIE_NAME];

                // The user has not logged in yet
                if (jwt == null)
                {
                    return Unauthorized("You are unauthorized to access this endpoint");
                }

                // Verify that the token is still valid
                var token = _jwtService.Verify(jwt);

                // Make sure the current user has access to the requested user
                token.Payload.TryGetValue(ClaimTypes.NameIdentifier, out var storedUserId);
                var currentUserIdStr = storedUserId as string;

                // If the user is neither an admin or getting their info
                Guid.TryParse(currentUserIdStr, out var currentUserGuid);

                var isAdmin = await _userService.IsAdmin(currentUserGuid);
                if (!isAdmin && currentUserIdStr != userId.ToString())
                {
                    return Unauthorized("The current user is not authorized to access this endpoint");
                }

                var user = await _userService.GetUserAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            catch (Exception e)
            {
                var message = $"There was an error getting the user with ID {userId}: {e.Message}";
                _logger.LogError(message, e.StackTrace);
                return Problem(message);
            }
        }

        /// <inheritdoc/>
        [HttpPut("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutAsync(Guid userId, [FromBody] UserUpdateDto userPutDto)
        {
            try
            {
                // Get the JWT cookie
                var jwt = Request.Cookies[GlobalConstants.JWT_TOKEN_COOKIE_NAME];

                // The user has not logged in yet
                if (jwt == null)
                {
                    return Unauthorized("You are unauthorized to access this endpoint");
                }
                
                // Verify that the token is still valid
                var token = _jwtService.Verify(jwt);

                // Make sure the current user has access to the requested user
                token.Payload.TryGetValue(ClaimTypes.NameIdentifier, out var storedUserId);
                var currentUserIdStr = storedUserId as string;

                Guid.TryParse(currentUserIdStr, out var currentUserGuid);
                var isAdmin = await _userService.IsAdmin(currentUserGuid);

                // If the user is neither an admin or getting their info, return
                if (!isAdmin && currentUserIdStr != userId.ToString())
                {
                    return Unauthorized("The current user is not authorized to access this endpoint");
                }

                if (userPutDto.Email != null && !_helperService.IsEmailValid(userPutDto.Email))
                {
                    return BadRequest("Invalid Email");
                }

                var res = await _userService.UpdateUserAsync(userId, userPutDto);
                if (res == null)
                {
                    return NotFound($"The user with the ID {userId} does not exist");
                }
                return Ok(res);
            }
            catch (Exception e)
            {
                var message = $"There was an error creating the user: {e.Message}";
                _logger.LogError(message, e.StackTrace);
                return Problem(message, statusCode: 500);
            }
        }

        /// <inheritdoc/>
        [HttpDelete("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid userId)
        {
            try
            {
                // Get the JWT cookie
                var jwt = Request.Cookies[GlobalConstants.JWT_TOKEN_COOKIE_NAME];

                // The user has not logged in yet
                if (jwt == null)
                {
                    return Unauthorized("You are unauthorized to access this endpoint");
                }
                
                // Verify that the token is still valid
                var token = _jwtService.Verify(jwt);

                // Make sure the current user has access to the requested user
                token.Payload.TryGetValue(ClaimTypes.NameIdentifier, out var storedUserId);
                var currentUserIdStr = storedUserId as string;

                Guid.TryParse(currentUserIdStr, out var currentUserGuid);
                var isAdmin = await _userService.IsAdmin(currentUserGuid);

                // If the user is neither an admin or getting their info, return
                if (!isAdmin && currentUserIdStr != userId.ToString())
                {
                    return Unauthorized("The current user is not authorized to access this endpoint");
                }

                var result = await _userService.DeleteUserAsync(userId);
                if (result)
                {
                    return Ok($"Successfully deleted user {userId}");
                }
                return NotFound($"User with ID {userId} does not exist");
            }
            catch (Exception e)
            {
                var message = $"There was a problem deleting the user with ID {userId}: {e.Message}";
                _logger.LogError(message, e.StackTrace);
                return Problem(message, statusCode: 500);
            }
        }
    }
}
