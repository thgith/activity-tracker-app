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
        private readonly IMapper _mapper;

        public UserService(IDataContext dataContext, IMapper mapper)
        {
            _dbContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Gets all active users.
        /// </summary>
        /// <returns>All the active users.</returns>
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            // AsNoTracking() since we are not modifying the result we get
            // Excludes deleted users
            var users = await _dbContext.Users
                .AsNoTracking()
                .Where(x => x.DateDeleted == null)
                .OrderBy(x => x.DateJoined)
                .ToListAsync();       
            
            // Convert entity to DTO and return
            return users.Select(x => _mapper.Map<UserDto>(x));
        }

        /// <summary>
        /// Gets the active user with the given ID.
        /// </summary>
        /// <param name="userId">The ID of the user to get.</param>
        /// <returns>The requested user.</returns>
        public async Task<UserDto> GetUserAsync(Guid userId)
        {
            var user = await _getActiveUser(userId);

            // Convert entity to DTO and return
            return _mapper.Map<UserDto>(user);
        }

        /// <summary>
        /// Creates the user.
        /// </summary>
        /// <param name="userPostDto">The user object.</param>
        /// <returns>The created user.</returns>
        public async Task<UserPostDto> CreateUserAsync(UserPostDto userPostDto)
        {
            // Auto set join date
            userPostDto.DateJoined = DateTime.UtcNow;

            // Convert DTO to entity
            var user =  _mapper.Map<User>(userPostDto);

            // Add user
            var newUser = await _dbContext.Users.AddAsync(user);

            // Save to DB
            await _dbContext.SaveChangesAsync();

            return userPostDto;
        }
        
        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="userId">The ID of the user to update.</param>
        /// <param name="userPutDto">The user object.</param>
        /// </summary>
        /// <returns>The updated user.</returns>
        public async Task<UserPutDto> UpdateUserAsync(Guid userId, UserPutDto userPutDto)
        {
            // Get active user with ID
            var user = await _getActiveUser(userId);
            
            // Return if user doesn't exist (or the user was soft deleted)
            if (user == null)
            {
                return null;
            }

            // Update fields
            user.FirstName = userPutDto.FirstName;
            user.LastName = userPutDto.LastName;
            user.Email = userPutDto.Email;
            user.Password = userPutDto.Password;

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
        public async Task<bool> DeleteUserAsync(Guid userId)
        {
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
            return await _dbContext.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Gets the active user with the ID.
        /// </summary>
        /// <param name="userId">The ID of the user to get.</param>
        /// <returns>The active user with the ID.</returns>
        private async Task<User> _getActiveUser(Guid userId)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(x => x.UserId == userId && x.DateDeleted == null);
        }
    }
}