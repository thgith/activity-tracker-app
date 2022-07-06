using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ActivityTrackerApp.Controllers;

/// <summary>
/// User endpoints. 
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

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllUsersAsync()
    {
        async Task<IActionResult> GetAllUsersPartialAsync(Guid currUserGuid)
        {
            var usersDtos = await userService.GetAllUsersAsync(currUserGuid);
            return Ok(usersDtos);
        }
        return await checkAuthAndPerformAction(Request, GetAllUsersPartialAsync);
    }

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
            var user = await userService.GetUserAsync(currUserGuid, userId);
            if (user == null)
            {
                return Problem($"The user with the ID {userId} does not exist", statusCode: StatusCodes.Status404NotFound);
            }
            return Ok(user);
        }
        return await checkAuthAndPerformAction(Request, GetUserPartialAsync);
    }

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
                return Problem("Invalid Email", statusCode: StatusCodes.Status400BadRequest);
            }

            // -- Update user --
            var user = await userService.UpdateUserAsync(currUserId, userId, userPutDto);
            if (user == null)
            {
                return Problem($"The user with the ID {userId} does not exist", statusCode: StatusCodes.Status404NotFound);
            }
            return Ok(user);
        }
        return await checkAuthAndPerformAction(Request, UpdateUserPartialAsync);
    }

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
            var isSuccess = await userService.DeleteUserAsync(currUserId, userId);
            if (!isSuccess)
            {
                return Problem($"User with ID {userId} does not exist", statusCode: StatusCodes.Status404NotFound);
            }
            return Ok($"Successfully deleted user {userId}");
        }
        return await checkAuthAndPerformAction(Request, DeleteUserPartialAsync);
    }


    [HttpPut("{userId}/changePassword")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ChangePasswordAsync(Guid userId, [FromBody] UserChangePasswordDto userDto)
    {
        async Task<IActionResult> ChangePasswordPartialAsync(Guid currUserGuid)
        {
            // Make sure the old password is correct
            var useGetDtoWithToken = await userService.AuthenticateUserAsync(new UserLoginDto()
            {
                Email = userDto.Email,
                Password = userDto.OldPassword
            });

            if (useGetDtoWithToken == null)
            {
                return Problem("Incorrect credentials", statusCode: StatusCodes.Status400BadRequest);
            }

            var successful = await userService.ChangePassword(currUserGuid, userDto.NewPassword);
            if (!successful)
            {
                return Problem($"There was a problem changing passwords for user {currUserGuid}");
            }
            return Ok(successful);
        }
        return await checkAuthAndPerformAction(Request, ChangePasswordPartialAsync);
    }
}
