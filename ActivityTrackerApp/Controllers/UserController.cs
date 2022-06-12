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
    public class UserController : ApiControllerBase<UserController>
    {
        private readonly IHelperService _helperService;

        public UserController(
            IHelperService helperService,
            IUserService userService,
            IJwtService jwtService,
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
        public async Task<IActionResult> GetAllUsersAsync()
        {
            async Task<IActionResult> GetAllUsersPartialAsync(Guid currUserGuid)
            {
                // -- Get all users --
                var usersDtos = await userService.GetAllUsersAsync(currUserGuid);
                return Ok(usersDtos);
            }
            return await checkAuthAndPerformAction(Request, GetAllUsersPartialAsync);            
        }

        /// <inheritdoc/>
        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserAsync(Guid userId)
        {
            async Task<IActionResult> GetUserPartialAsync(Guid currUserGuid)
            {
                // -- Get user --
                var user = await userService.GetUserAsync(currUserGuid, userId);
                if (user == null)
                {
                    return NotFound($"The user with the ID {userId} does not exist");
                }
                return Ok(user);
            }
            return await checkAuthAndPerformAction(Request, GetUserPartialAsync);
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
            async Task<IActionResult> UpdateUserPartialAsync(Guid currUserId)
            {
                // Check email
                if (userPutDto.Email != null && !_helperService.IsEmailValid(userPutDto.Email))
                {
                    return BadRequest("Invalid Email");
                }

                // -- Update user --
                var user = await userService.UpdateUserAsync(currUserId, userId, userPutDto);
                if (user == null)
                {
                    return NotFound($"The user with the ID {userId} does not exist");
                }
                return Ok(user);
            }
            return await checkAuthAndPerformAction(Request, UpdateUserPartialAsync);
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
            async Task<IActionResult> DeleteUserPartialAsync(Guid currUserId)
            {
                // -- Delete user --
                var isSuccess = await userService.DeleteUserAsync(currUserId, userId);
                if (!isSuccess)
                {
                    return NotFound($"User with ID {userId} does not exist");
                }
                return Ok($"Successfully deleted user {userId}");
            }
            return await checkAuthAndPerformAction(Request, DeleteUserPartialAsync);

        }
    }
}
