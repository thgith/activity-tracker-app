namespace ActivityTrackerApp.Controllers
{
    ///<summary>
    /// An activity. 
    /// Endpoint will be: api/v1/User
    ///</summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : IUserController
    {
        IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        ///<summary>
        /// Get the user with the given ID.
        ///</summary>
        ///<returns>Task of the user.</returns>
        [HttpGet("{userId}")]
        public async Task<ActionResult<UserDto>> GetUser(Guid userId)
        {
            return null;
        }

        ///<summary>
        /// Create the new user.
        ///</summary>
        ///<returns>Task of the newly created user.</returns>
        ///<param name="userDto">The user model for the create.</param>
        [HttpPost]
        public async Task<IActionResult<UserDto>> CreateUser(UserDto userDto)
        {
            return null;
        }

        ///<summary>
        /// Update the user.
        ///</summary>
        ///<returns>Task of the updated user.</returns>
        ///<param name="userDto">The user model for the update.</param>
        [HttpPut("{userId}")]
        public async Task<ActionResult<UserDto>> UpdateUser(UserDto userDto)
        {
            return null;
        }

        ///<summary>
        /// Delete the user with the given ID.
        ///<summary>
        ///<param name="userId">The GUID of the user to delete.</param>
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            return null;
        }
    }
}
