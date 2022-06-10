using ActivityTrackerApp.Dtos;
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
        private readonly IHelperMethods _helperService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IUserService userService,
            IHelperMethods helperService,
            IMapper mapper,
            ILogger<UserController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _helperService = helperService ?? throw new ArgumentNullException(nameof(helperService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <inheritdoc/>
        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <inheritdoc/>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> PostAsync(UserPostDto userPostDto)
        {
            // Requirement annotations 
            if (!_helperService.IsEmailValid(userPostDto.Email))
            {
                BadRequest("Invalid Email");
            }

            if (await _userService.IsEmailTaken(userPostDto.Email))
            {
                BadRequest("Email already taken.");
            }

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

        /// <inheritdoc/>
        [HttpPut("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutAsync(Guid userId, UserPutDto userPutDto)
        {
            if (userPutDto.Email != null && !_helperService.IsEmailValid(userPutDto.Email))
            {
                return BadRequest("Invalid Email");
            }

            try
            {
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
