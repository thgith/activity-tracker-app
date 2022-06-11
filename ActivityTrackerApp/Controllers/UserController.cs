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
    public class UserController : ApiControllerBase<UserController>, IUserController
    {
        private readonly IHelperService _helperService;

        public UserController(
            IUserService userService,
            IJwtService jwtService,
            IHelperService helperService,
            ILogger<UserController> logger) : base(userService, jwtService, logger)
        {
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
        }

        /// <inheritdoc/>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<UserGetDto>>> GetAllUsersAsync()
        {
            try
            {
                // Check permissions
                var authErr = await checkAuthenticatedAndAuthorized(Request, null);
                if (authErr != null)
                {
                    return authErr;
                }

                // Get all users
                var usersDtos = await userService.GetAllUsersAsync();
                return Ok(usersDtos);
            }
            catch (Exception e)
            {
                var message = $"There was an error getting the users";
                logger.LogError(message, e.Message, e.StackTrace);
                return Problem(message, statusCode: 500);
            }
        }

        /// <inheritdoc/>
        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserGetDto>> GetUserAsync(Guid userId)
        {
            try
            {
                // Check permissions
                var authErr = await checkAuthenticatedAndAuthorized(Request, userId);
                if (authErr != null)
                {
                    return authErr;
                }

                // Get user
                var user = await userService.GetUserAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            catch (Exception e)
            {
                var message = $"There was an error getting the user with ID {userId}";
                logger.LogError(message, e.Message, e.StackTrace);
                return Problem(message);
            }
        }

        /// <inheritdoc/>
        [HttpPut("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserAsync(
            Guid userId,
            [FromBody] UserUpdateDto userPutDto)
        {
            try
            {
                // Check permissions
                var authErr = await checkAuthenticatedAndAuthorized(
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
                var res = await userService.UpdateUserAsync(userId, userPutDto);
                if (res == null)
                {
                    return NotFound($"The user with the ID {userId} does not exist");
                }
                return Ok(res);
            }
            catch (Exception e)
            {
                var message = "There was an error creating the user";
                logger.LogError(message, e.Message, e.StackTrace);
                return Problem(message, statusCode: 500);
            }
        }

        /// <inheritdoc/>
        [HttpDelete("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUserAsync(Guid userId)
        {
            try
            {
                // Check permissions
                var authErr = await checkAuthenticatedAndAuthorized(
                                        Request,
                                        userId,
                                        allowIfSameUser: true);
                if (authErr != null)
                {
                    return authErr;
                }

                // Delete user
                var result = await userService.DeleteUserAsync(userId);
                if (result)
                {
                    return Ok($"Successfully deleted user {userId}");
                }
                return NotFound($"User with ID {userId} does not exist");
            }
            catch (Exception e)
            {
                var message = $"There was a problem deleting the user with ID {userId}";
                logger.LogError(message, e.Message, e.StackTrace);
                return Problem(message, statusCode: 500);
            }
        }
    }
}
