using ActivityTrackerApp.Constants;
using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ActivityTrackerApp.Services
{
    /// <summary>
    /// User service.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IDataContext _dbContext;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public UserService(
            IDataContext dataContext,
            IJwtService jwtService,
            IConfiguration config,
            IMapper mapper)
        {
            _dbContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<EntityWithToken<UserRegisterDto>> RegisterUserAsync(UserRegisterDto userRegisterDto)
        {
            var user = await _createUserAsync(userRegisterDto);
            return new EntityWithToken<UserRegisterDto>()
            {
                Entity = userRegisterDto,
                Token = _jwtService.GenerateJwtToken(user)
            };
        }

        public async Task<EntityWithToken<UserLoginDto>> AuthenticateUserAsync(UserLoginDto userLoginDto)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(x => 
                x.Email == userLoginDto.Email &&
                x.DateDeleted == null);

            // User with email doesn't exist
            if (user == null)
            {
                return null;
            }

            // Password is invalid
            if (!BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.PasswordHash))
            {
                return null;
            }

            return new EntityWithToken<UserLoginDto>()
            {
                Entity = userLoginDto,
                Token = _jwtService.GenerateJwtToken(user)
            };
        }

        /// <summary>
        /// Gets all active users.
        /// </summary>
        /// <returns>All the active users.</returns>
        public async Task<IEnumerable<UserGetDto>> GetAllUsersAsync(Guid currUserId)
        {
            // Check permissions
            if (!(await IsAdmin(currUserId)))
            {
                throw new UnauthorizedAccessException("The current user is not authorized to get all users");
            }

            // Excludes deleted users
            var users = await _dbContext.Users
                .Where(x => x.DateDeleted == null)
                .OrderBy(x => x.DateJoined)
                .ToListAsync();       
            
            // Convert entity to DTO and return
            return users.Select(x => _mapper.Map<UserGetDto>(x));
        }

        /// <summary>
        /// Gets the active user with the given ID.
        /// </summary>
        /// <param name="userId">The ID of the user to get.</param>
        /// <returns>The requested user.</returns>
        public async Task<UserGetDto> GetUserAsync(Guid currUserId, Guid userId)
        {
            // Check permissions
            if (currUserId != userId && !(await IsAdmin(currUserId)))
            {
                throw new UnauthorizedAccessException("The current user is not authorized to get this user");
            }

            var user = await _getActiveUser(userId);

            // Convert entity to DTO and return
            return _mapper.Map<UserGetDto>(user);
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="userId">The ID of the user to update.</param>
        /// <param name="userPutDto">The user object.</param>
        /// </summary>
        /// <returns>The updated user.</returns>
        public async Task<UserUpdateDto> UpdateUserAsync(Guid currUserId, Guid userId, UserUpdateDto userPutDto)
        {
            // Check permissions
            if (currUserId != userId && !(await IsAdmin(currUserId)))
            {
                throw new UnauthorizedAccessException("The current user is not authorized to get this user");
            }

            // Get active user with ID
            var user = await _getActiveUser(userId);
            
            // Return if user doesn't exist (or the user was soft deleted)
            if (user == null)
            {
                return null;
            }

            // Update fields
            if (userPutDto.FirstName != null)
            {
                user.FirstName = userPutDto.FirstName;
            }

            if (userPutDto.LastName != null)
            {
                user.LastName = userPutDto.LastName;
            }

            if (userPutDto.Email != null)
            {
                user.Email = userPutDto.Email;
            }

            if (userPutDto.Password != null)
            {
                user.PasswordHash = _hashPassword(userPutDto.Password);
            }

            // Save to DB
            await _dbContext.SaveChangesAsync();

            return userPutDto;
        }

        /// <summary>
        /// Soft deletes the user with the ID.
        /// </summary>
        /// <param name="userId">The ID of the user to delete.</param>
        /// <returns>
        /// <c>true</c> if the delete was successful, <c>false</c> otherwise.
        /// </returns>
        public async Task<bool> DeleteUserAsync(Guid currUserId, Guid userId)
        {
            // Check permissions
            if (currUserId != userId && !(await IsAdmin(currUserId)))
            {
                throw new UnauthorizedAccessException("The current user is not authorized to get this user");
            }

            // Get active user with ID
            var user = await _getActiveUser(userId);

            // Return if user doesn't exist (or the user was already soft deleted)
            if (user == null)
            {
                return false;
            }

            // Soft delete the user
            user.DateDeleted = DateTime.UtcNow;

            // Save to DB
            // SaveChangesAsync returns the number of entries written to the DB
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsAdmin(Guid userId)
        {
            return await _dbContext.Users
                .AnyAsync(x => 
                    x.Id == userId && 
                    x.DateDeleted == null &&
                    x.Role == Roles.ADMIN);
        }

        /// <remarks>This includes emails that are part of deactiviated users.</remarks>
        public async Task<bool> IsEmailTaken(string email)
        {
            return await _dbContext.Users.AnyAsync(x => x.Email == email);
        }

        private async Task<User> _createUserAsync(UserRegisterDto userRegisterDto)
        {
            // Convert DTO to entity
            var user =  _mapper.Map<User>(userRegisterDto);

            // Auto set join date
            user.DateJoined = DateTime.UtcNow;

            // Hash password for security
            user.PasswordHash = _hashPassword(userRegisterDto.Password);

            user.Role = Roles.MEMBER;

            // Add user
            await _dbContext.Users.AddAsync(user);

            // Save to DB
            await _dbContext.SaveChangesAsync();

            return user;
        }

        /// <summary>
        /// Gets the active user with the ID.
        /// </summary>
        /// <param name="userId">The ID of the user to get.</param>
        /// <returns>The active user with the ID.</returns>
        private async Task<User> _getActiveUser(Guid userId)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(x => x.Id == userId && x.DateDeleted == null);
        }

        private string _hashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, _config[GlobalConstants.PASSWORD_HASH_SALT_KEY_NAME]);
        }
    }
}