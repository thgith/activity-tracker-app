using ActivityTrackerApp.Constants;
using ActivityTrackerApp.Database;
using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Entities;
using ActivityTrackerApp.Exceptions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ActivityTrackerApp.Services;

/// <inheritdoc/>
public class UserService : IUserService
{
    private readonly IDataContext _dbContext;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _config;
    private readonly IMapper _mapper;

    /// <summary>
    /// Constructor.
    /// </summary>
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

    /// <inheritdoc/>
    public async Task<EntityWithToken<UserGetDto>> RegisterUserAsync(UserRegisterDto userRegisterDto)
    {
        var user = await _createUserAsync(userRegisterDto);
        return new EntityWithToken<UserGetDto>()
        {
            Entity = _mapper.Map<UserGetDto>(user),
            Token = _jwtService.GenerateJwtToken(user)
        };
    }

    /// <inheritdoc/>
    public async Task<EntityWithToken<UserGetDto>> AuthenticateUserAsync(UserLoginDto userLoginDto)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => 
            x.Email.ToLower() == userLoginDto.Email.ToLower() &&
            x.DeletedDateUtc == null);

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

        return new EntityWithToken<UserGetDto>()
        {
            Entity = _mapper.Map<UserGetDto>(user),
            Token = _jwtService.GenerateJwtToken(user)
        };
    }

    /// <inheritdoc/>
    public async Task<bool> ChangePassword(Guid currUserId, string newPassword)
    {
        var user = await _getActiveUser(currUserId);

        // Return if user doesn't exist (or the user was soft deleted)
        if (user == null)
        {
            return false;
        }

        user.PasswordHash = _hashPassword(newPassword);
        
        await _dbContext.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<UserGetDto>> GetAllUsersAsync(Guid currUserId)
    {
        // Check permissions
        if (!(await IsAdmin(currUserId)))
        {
            throw new ForbiddenException();
        }

        // Excludes deleted users
        var users = await _dbContext.Users
            .Where(x => x.DeletedDateUtc == null)
            .OrderBy(x => x.JoinDateUtc)
            .ToListAsync();       
        
        // Convert entity to DTO and return
        return users.Select(x => _mapper.Map<UserGetDto>(x));
    }

    /// <inheritdoc/>
    public async Task<UserGetDto> GetUserAsync(Guid currUserId, Guid userId)
    {
        // Check permissions
        if (currUserId != userId && !(await IsAdmin(currUserId)))
        {
            throw new ForbiddenException();
        }

        var user = await _getActiveUser(userId);

        // Convert entity to DTO and return
        return _mapper.Map<UserGetDto>(user);
    }

    /// <inheritdoc/>
    public async Task<UserGetDto> UpdateUserAsync(Guid currUserId, Guid userId, UserUpdateDto userPutDto)
    {
        if (userPutDto == null)
        {
            throw new InvalidDataException();
        }

        // Check permissions
        if (currUserId != userId && !(await IsAdmin(currUserId)))
        {
            throw new ForbiddenException();
        }

        // Get active user with ID
        var user = await _getActiveUser(userId);
        
        // Return if user doesn't exist (or the user was soft deleted)
        if (user == null)
        {
            return null;
        }

        var hasChange = false;
        // Update fields
        if (userPutDto.FirstName != null)
        {
            user.FirstName = userPutDto.FirstName;
            hasChange = true;
        }

        if (userPutDto.LastName != null)
        {
            user.LastName = userPutDto.LastName;
            hasChange = true;
        }

        if (userPutDto.Email != null && userPutDto.Email.ToLower() != user.Email.ToLower())
        {
            if (await IsEmailTaken(userPutDto.Email))
            {
                throw new InvalidDataException("This email is already in use");
            }
            user.Email = userPutDto.Email.ToLower();
            hasChange = true;
        }

        if (userPutDto.Password != null)
        {
            user.PasswordHash = _hashPassword(userPutDto.Password);
            hasChange = true;
        }

        // Save to DB
        if (hasChange)
        {
            await _dbContext.SaveChangesAsync();
        }

        return _mapper.Map<UserGetDto>(user);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteUserAsync(Guid currUserId, Guid userId)
    {
        // Check permissions
        if (currUserId != userId && !(await IsAdmin(currUserId)))
        {
            throw new ForbiddenException();
        }

        // Get active user with ID
        var user = await _getActiveUser(userId);

        // Return if user doesn't exist (or the user was already soft deleted)
        if (user == null)
        {
            return false;
        }

        // Soft delete the user
        var utcNow = DateTime.UtcNow;
        var deleteDateUtc = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, utcNow.Hour, utcNow.Minute, utcNow.Second, DateTimeKind.Utc);
        user.DeletedDateUtc = deleteDateUtc;

        // Soft delete all the activities associated with the user
        var activitiesToDelete = _dbContext.Activities.Where(x => x.OwnerId == userId && x.DeletedDateUtc == null);
        foreach (var activity in activitiesToDelete)
        {
            // Soft delete all sessions associated with the activities
            var sessionsToDelete = _dbContext.Sessions.Where(x => x.ActivityId == activity.Id && x.DeletedDateUtc == null);
            foreach (var session in sessionsToDelete)
            {
                session.DeletedDateUtc = deleteDateUtc;
            }
            activity.DeletedDateUtc = deleteDateUtc;
        }

        // Save to DB
        // SaveChangesAsync returns the number of entries written to the DB
        await _dbContext.SaveChangesAsync();

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> IsAdmin(Guid userId)
    {
        return await _dbContext.Users
            .AnyAsync(x => 
                x.Id == userId && 
                x.DeletedDateUtc == null &&
                x.Role == Roles.ADMIN);
    }

    /// <inheritdoc/>
    public async Task<bool> IsEmailTaken(string email)
    {
        return await _dbContext.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower());
    }

    /// <summary>
    /// Creates the user.
    /// </summary>
    /// <param name="userRegisterDto">The registration data.</param>
    /// <returns>The created user.</returns>
    private async Task<User> _createUserAsync(UserRegisterDto userRegisterDto)
    {
        if (userRegisterDto == null)
        {
            throw new InvalidDataException("User registration object cannot be null");
        }
        
        // Convert DTO to entity
        var user =  _mapper.Map<User>(userRegisterDto);

        // Always store email in lowercase
        user.Email = user.Email.ToLower();
        
        // Auto set join date. Recreate to get rid of ms
        var utcNow = DateTime.UtcNow;
        user.JoinDateUtc = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, utcNow.Hour, utcNow.Minute, utcNow.Second, DateTimeKind.Utc);

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
            .FirstOrDefaultAsync(x => x.Id == userId && x.DeletedDateUtc == null);
    }

    private string _hashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}