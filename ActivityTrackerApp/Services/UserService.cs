using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ActivityTrackerApp.Services
{
    /// <summary>
    /// User service.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IDataContext _dbContext;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public UserService(
            IDataContext dataContext,
            IConfiguration config,
            IMapper mapper)
        {
            _dbContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<User> AuthenticateAsync(UserPutDto userPutDto)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(x => 
                x.Email == userPutDto.Email &&
                x.DateDeleted == null);

            // User with email doesn't exist
            if (user == null)
            {
                return null;
            }

            // Password is invalid
            if (!BCrypt.Net.BCrypt.Verify(userPutDto.Password, user.PasswordHash))
            {
                return null;
            }

            return user;
        }

        public string GenerateJwtToken(User user)
        {
            // Get key in config
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("JwtConfig:Secret"));
            
            // Create credentials with key and selected signing algorithm
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Claims are to store data about user/process
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                _config["JwtConfig:Issuer"],
                _config["JwtConfig:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials);
            
            return new JwtSecurityTokenHandler().WriteToken(token);
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
            await _createUserAsync(userPostDto);
            return userPostDto;
        }

        public async Task<UserPostDtoWithToken> RegisterUserAsync(UserPostDto userPostDto)
        {
            var user = await _createUserAsync(userPostDto);
            var token = GenerateJwtToken(user);
            return new UserPostDtoWithToken()
            {
                User = userPostDto,
                Token = token
            };
        }

        private async Task<User> _createUserAsync(UserPostDto userPostDto)
        {
            // Convert DTO to entity
            var user =  _mapper.Map<User>(userPostDto);

            // Auto set join date
            user.DateJoined = DateTime.UtcNow;

            // Hash password for security
            // TODO add salt
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userPostDto.Password);

            // TODO make Roles class
            user.Role = "member";

            // Add user
            await _dbContext.Users.AddAsync(user);

            // Save to DB
            await _dbContext.SaveChangesAsync();

            return user;
        }


        public async Task<bool> IsEmailTaken(string email)
        {
            return await _dbContext.Users.AnyAsync(x => x.Email == email);
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
            if (user.FirstName != null)
            {
                user.FirstName = userPutDto.FirstName;
            }

            if (user.LastName != null)
            {
                user.LastName = userPutDto.LastName;
            }

            if (user.Email != null)
            {
                user.Email = userPutDto.Email;
            }

            if (user.PasswordHash != null)
            {
                user.PasswordHash = userPutDto.Password;
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
                .FirstOrDefaultAsync(x => x.Id == userId && x.DateDeleted == null);
        }
    }
}