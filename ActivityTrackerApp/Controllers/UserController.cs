using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Entities;
using ActivityTrackerApp.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ActivityTrackerApp.Controllers
{
    /// <summary>
    /// An activity. 
    /// Endpoint will be: api/v1/User
    /// </summary>
    [Route("api/v1/User")] // api/v1/[controller] didn't work
    public class UserController : ApiControllerBase, IUserController
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IUserService userService,
            IMapper mapper,
            ILogger<UserController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves users.
        /// </summary>
        /// <returns>List of users</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllAsync()
        {
            try
            {
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

        /// <summary>
        /// Get the user with the given ID.
        /// </summary>
        /// <returns>Task of the user.</returns>
        [HttpGet("{userId}")]
        public async Task<ActionResult<UserDto>> GetAsync(Guid userId)
        {
            try
            {
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

        /// <summary>
        /// Create the new user.
        /// </summary>
        /// <returns>Task of the newly created user.</returns>
        /// <param name="userDto">The user model for the create.</param>
        [HttpPost]
        public async Task<ActionResult> PostAsync(UserPostDto userPostDto)
        {
            try
            {
                var res = await _userService.CreateUserAsync(userPostDto);
                return Ok(res);
            }
            catch (Exception e)
            {
                var message = $"There was an error creating the user: {e.Message}";
                _logger.LogError(message, e.StackTrace);
                return Problem(message, statusCode: 500);
            }
        }

        /// <summary>
        /// Update the user.
        /// </summary>
        /// <returns>Task of the updated user.</returns>
        /// <param name="userDto">The user model for the update.</param>
        [HttpPut("{userId}")]
        public async Task<IActionResult> PutAsync(Guid userId, UserPutDto userPutDto)
        {
            try
            {
                var res = await _userService.UpdateUserAsync(userId, userPutDto);
                return Ok(res);
            }
            catch (Exception e)
            {
                var message = $"There was an error creating the user: {e.Message}";
                _logger.LogError(message, e.StackTrace);
                return Problem(message, statusCode: 500);
            }
        }

        /// <summary>
        /// Delete the user with the given ID.
        /// <summary>
        /// <param name="userId">The GUID of the user to delete.</param>
        [HttpDelete("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> DeleteAsync(Guid userId)
        {
            try
            {
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
