using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Entities;
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
        private readonly IHelperMethods _helperService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserController> _logger;

        public AuthController(
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
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RegisterAsync([FromBody] UserPostDto userPostDto)
        {
            try
            {
                // Note: model annotation handle error checking for requirements
                if (!_helperService.IsEmailValid(userPostDto.Email))
                {
                    return BadRequest("Invalid Email");
                }

                if (await _userService.IsEmailTaken(userPostDto.Email))
                {
                    return BadRequest("Email already taken");
                }

                var userPostDtoWithToken = await _userService.RegisterUserAsync(userPostDto);
        
                return Ok(userPostDtoWithToken.Token);
            }
            catch (Exception e)
            {
                var message = $"There was an error logging in: {e.Message}";
                _logger.LogError(message, e.StackTrace);
                return Problem(message, statusCode: 500);
            }
        }
    }
}