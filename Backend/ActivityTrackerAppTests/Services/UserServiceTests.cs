using ActivityTrackerApp.Constants;
using ActivityTrackerApp.Database;
using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Entities;
using ActivityTrackerApp.Exceptions;
using ActivityTrackerApp.Services;
using static ActivityTrackerAppTests.Fixtures.TestFixtures;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MockQueryable.Moq;
using Moq;

namespace ActivityTrackerAppTests;

// NOTE: Prob should add more checks to check side effects (call count, etc),
//       but this is fine for now
[TestClass]
public class UserServiceTests
{
    private static User _janeUser;
    private static User _johnUser;
    private static User _judyUser;
    private static List<User> _usersData;
    private static Mock<DbSet<User>> _usersDbSetMock;
    private static Mock<IDataContext> _dbContextMock;
    private static Mock<IConfiguration> _configMock;
    private static Mock<IJwtService> _jwtServiceMock;
    private static Mock<IMapper> _mapperMock;

    // Called before all tests
    [ClassInitialize()]
    public static void InitializeClass(TestContext context)
    {
    }

    // Called before each test
    [TestInitialize]
    public void InitializeTests()
    {
        // Init users here b/c they may change throughout each user test
        _janeUser = GenerateJaneUser();
        _johnUser = GenerateJohnUser();
        // Deleted user
        _judyUser = GenerateJudyUser();
        _usersData = new List<User> { _janeUser, _johnUser, _judyUser };

        // Set up mock objects
        _usersDbSetMock = _usersData.AsQueryable().BuildMockDbSet();
        _dbContextMock = new Mock<IDataContext>();
        _dbContextMock.Setup(x => x.Users)
                .Returns(_usersDbSetMock.Object);
        _jwtServiceMock = new Mock<IJwtService>();
        _configMock = new Mock<IConfiguration>();
        _mapperMock = new Mock<IMapper>();
    }

    #region GetAllUsersAsync
    [TestMethod]
    public async Task GetAllUsersAsync_Admin_Ok()
    {
        // -- Arrange --
        _mapperMock.Setup(x => x.Map<UserGetDto>(_janeUser))
                    .Returns(
                        new UserGetDto
                        {
                            FirstName = JANE_FIRST_NAME,
                            LastName = COMMON_LAST_NAME,
                            Email = JANE_EMAIL,
                            JoinDateUtc = JANE_JOIN_DATE_UTC
                        }
                    );
        _mapperMock.Setup(x => x.Map<UserGetDto>(_johnUser))
                    .Returns(
                        new UserGetDto
                        {
                            FirstName = JOHN_FIRST_NAME,
                            LastName = COMMON_LAST_NAME,
                            Email = JOHN_EMAIL,
                            JoinDateUtc = JOHN_JOIN_DATE_UTC
                        }
                    );

        var userService = new UserService(
            _dbContextMock.Object,
            _jwtServiceMock.Object,
            _configMock.Object,
            _mapperMock.Object);

        // -- Act --
        var users = await userService.GetAllUsersAsync(JANE_USER_GUID);

        // -- Assert --
        Assert.IsNotNull(users);
        Assert.AreEqual(users.Count(), 2);
        var usersList = users.ToList();

        // This list should be ordered by join date, so the users should be in this order
        // Didn't get deleted user
        _assertUsersSame(usersList[0], JANE_FIRST_NAME, COMMON_LAST_NAME, JANE_EMAIL);
        _assertUsersSame(usersList[1], JOHN_FIRST_NAME, COMMON_LAST_NAME, JOHN_EMAIL);
    }

    [TestMethod]
    [ExpectedException(typeof(ForbiddenException))]
    public async Task GetAllUsersAsync_NotAdmin_ThrowForbidden()
    {
        // -- Arrange --
        var userService = new UserService(
            _dbContextMock.Object,
            _jwtServiceMock.Object,
            _configMock.Object,
            _mapperMock.Object);

        // -- Act --
        var users = await userService.GetAllUsersAsync(JOHN_USER_GUID);
    }

    [TestMethod]
    public async Task GetUserAsync_Admin_AnotherUser_Ok()
    {
        // -- Arrange --
        _mapperMock.Setup(x => x.Map<UserGetDto>(_johnUser))
                    .Returns(
                        new UserGetDto
                        {
                            FirstName = JOHN_FIRST_NAME,
                            LastName = COMMON_LAST_NAME,
                            Email = JOHN_EMAIL,
                            JoinDateUtc = JOHN_JOIN_DATE_UTC
                        }
                    );

        var userService = new UserService(
            _dbContextMock.Object,
            _jwtServiceMock.Object,
            _configMock.Object,
            _mapperMock.Object);

        // -- Act --
        // Try to get John's info as admin Jane
        var user = await userService.GetUserAsync(JANE_USER_GUID, JOHN_USER_GUID);

        // -- Assert --
        Assert.IsNotNull(user);
        _assertUsersSame(user, JOHN_FIRST_NAME, COMMON_LAST_NAME, JOHN_EMAIL);
    }
    #endregion

    #region GetUserAsync
    [TestMethod]
    public async Task GetUserAsync_Admin_AnotherNonExistentUser_ReturnNull()
    {
        // -- Arrange --
        var userService = new UserService(
            _dbContextMock.Object,
            _jwtServiceMock.Object,
            _configMock.Object,
            _mapperMock.Object);

        // -- Act --
        // Try to get nonexistent as admin Jane
        var user = await userService.GetUserAsync(JANE_USER_GUID, Guid.NewGuid());

        // -- Assert --
        Assert.IsNull(user);
    }

    [TestMethod]
    public async Task GetUserAsync_Admin_AnotherDeletedUser_ReturnNull()
    {
        // -- Arrange --
        var userService = new UserService(
            _dbContextMock.Object,
            _jwtServiceMock.Object,
            _configMock.Object,
            _mapperMock.Object);

        // -- Act --
        // Try to get deleted user Judy's info as admin Jane
        var user = await userService.GetUserAsync(JANE_USER_GUID, JUDY_USER_GUID);

        // -- Assert --
        Assert.IsNull(user);
    }

    [TestMethod]
    public async Task GetUserAsync_NonAdmin_SameUser_Ok()
    {
        // -- Arrange --
        _mapperMock.Setup(x => x.Map<UserGetDto>(_johnUser))
                    .Returns(
                        new UserGetDto
                        {
                            FirstName = JOHN_FIRST_NAME,
                            LastName = COMMON_LAST_NAME,
                            Email = JOHN_EMAIL,
                            JoinDateUtc = JOHN_JOIN_DATE_UTC
                        }
                    );

        var userService = new UserService(
            _dbContextMock.Object,
            _jwtServiceMock.Object,
            _configMock.Object,
            _mapperMock.Object);

        // -- Act --
        // Try to get John's info as member John
        var user = await userService.GetUserAsync(JOHN_USER_GUID, JOHN_USER_GUID);

        // -- Assert --
        Assert.IsNotNull(user);
        _assertUsersSame(user, JOHN_FIRST_NAME, COMMON_LAST_NAME, JOHN_EMAIL);
    }

    [TestMethod]
    [ExpectedException(typeof(ForbiddenException))]
    public async Task GetUserAsync_NonAdmin_AnotherUser_ThrowForbidden()
    {
        // -- Arrange --
        _mapperMock.Setup(x => x.Map<UserGetDto>(_johnUser))
                    .Returns(
                        new UserGetDto
                        {
                            FirstName = JOHN_FIRST_NAME,
                            LastName = COMMON_LAST_NAME,
                            Email = JOHN_EMAIL,
                            JoinDateUtc = JOHN_JOIN_DATE_UTC
                        }
                    );

        var userService = new UserService(
            _dbContextMock.Object,
            _jwtServiceMock.Object,
            _configMock.Object,
            _mapperMock.Object);

        // -- Act --
        // Try to get John's info as member John
        var user = await userService.GetUserAsync(JOHN_USER_GUID, JANE_USER_GUID);
    }
    #endregion

    #region RegisterUserAsync
    // The model annotations and some ctrl checks handle a lot of the err
    // checking, so for now this is fine.
    [TestMethod]
    public async Task RegisterUserAsync_Ok()
    {
        var dummyToken = "dummytoken";
        // -- Arrange --
        _jwtServiceMock.Setup(m => m.GenerateJwtToken(It.IsAny<User>(), 300))
                        .Returns(dummyToken);
        _mapperMock.Setup(m => m.Map<User>(It.IsAny<UserRegisterDto>()))
                    .Returns(
                        new User
                        {
                            Id = Guid.NewGuid(),
                            FirstName = LILA_FIRST_NAME,
                            LastName = COMMON_LAST_NAME,
                            Email = LILA_EMAIL,
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(COMMON_OLD_PASSWORD),
                            // Technically we do more with this date, but this is close enough
                            JoinDateUtc = DateTime.UtcNow,
                            DeletedDateUtc = null,
                            Role = Roles.MEMBER
                        }
                    );
        _mapperMock.Setup(m => m.Map<UserGetDto>(It.IsAny<User>()))
                    .Returns(
                    new UserGetDto
                        {
                            FirstName = LILA_FIRST_NAME,
                            LastName = COMMON_LAST_NAME,
                            Email = LILA_EMAIL,
                            JoinDateUtc = DateTime.UtcNow
                        }
                    );

        var userService = new UserService(
            _dbContextMock.Object,
            _jwtServiceMock.Object,
            _configMock.Object,
            _mapperMock.Object);

        var registerUserDto = new UserRegisterDto
            {
                FirstName = LILA_FIRST_NAME,
                LastName = COMMON_LAST_NAME,
                Email = LILA_EMAIL,
                Password = COMMON_OLD_PASSWORD
            };

        // Add the user that is actually saved to our test data to check against later
        _usersDbSetMock.Setup(m => m.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                        .Callback((User user, CancellationToken _) => { _usersData.Add(user); });

        // -- Act --
        var returnedEntityWithToken = await userService.RegisterUserAsync(registerUserDto);
        var returnedEntity = ((EntityWithToken<UserGetDto>)returnedEntityWithToken).Entity;
        var returnedToken = ((EntityWithToken<UserGetDto>)returnedEntityWithToken).Token;

        // -- Assert --
        Assert.IsNotNull(returnedEntityWithToken);
        Assert.IsNotNull(returnedToken);

        // Check that the returned object is as expected
        _assertUsersSame(returnedEntity, LILA_FIRST_NAME, COMMON_LAST_NAME, LILA_EMAIL);
        Assert.AreEqual(dummyToken, returnedToken);

        _usersDbSetMock.Verify(m => m.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Once);
        
        // This saved user should have been added during the AddAsync callback
        Assert.AreEqual(_usersData.Count(), 4);
        var newUserLila = _usersData.Last();

        // Check that the data was saved to the DB correctly
        Assert.AreEqual(LILA_FIRST_NAME, newUserLila.FirstName);
        Assert.AreEqual(COMMON_LAST_NAME, newUserLila.LastName);
        Assert.AreEqual(LILA_EMAIL, newUserLila.Email);
        BCrypt.Net.BCrypt.Verify(COMMON_OLD_PASSWORD, newUserLila.PasswordHash);
        Assert.IsTrue(DatesEqualWithinSeconds((DateTime) newUserLila.JoinDateUtc, DateTime.UtcNow));
        Assert.IsNull(newUserLila.DeletedDateUtc);
        Assert.AreEqual(Roles.MEMBER, newUserLila.Role);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidDataException))]
    public async Task RegisterUserAsync_NullObject_ThrowInvalidData()
    {
        // -- Arrange --
        var userService = new UserService(
            _dbContextMock.Object,
            _jwtServiceMock.Object,
            _configMock.Object,
            _mapperMock.Object);

        // -- Act --
        var returnedEntityWithToken = await userService.RegisterUserAsync(null);
    }
    #endregion

    #region UpdateUserAsync
    [TestMethod]
    public async Task UpdateUserAsync_Admin_AnotherUser_Ok()
    {
        // -- Arrange --
        _mapperMock.Setup(m => m.Map<UserGetDto>(It.IsAny<User>()))
            .Returns(new UserGetDto
            {
                FirstName = NEW_FIRST_NAME,
                LastName = NEW_LAST_NAME,
                Email = NEW_EMAIL,
                JoinDateUtc = _johnUser.JoinDateUtc
            });
        var userService = new UserService(
            _dbContextMock.Object,
            _jwtServiceMock.Object,
            _configMock.Object,
            _mapperMock.Object);

        // -- Act --
        var returnedDto = await userService.UpdateUserAsync(
            JANE_USER_GUID,
            JOHN_USER_GUID,
            new UserUpdateDto
            {
                FirstName = NEW_FIRST_NAME,
                LastName = NEW_LAST_NAME,
                Email = NEW_EMAIL,
                Password = NEW_PASSWORD
            }
        );

        // -- Assert --
        Assert.IsNotNull(returnedDto);

        // Check that the returned user is as expected
        _assertUsersSame(returnedDto, NEW_FIRST_NAME, NEW_LAST_NAME, NEW_EMAIL);

        // Check that the returned user is as expected
        _dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Once);
        Assert.AreEqual(NEW_FIRST_NAME, _johnUser.FirstName);
        Assert.AreEqual(NEW_LAST_NAME, _johnUser.LastName);
        Assert.AreEqual(NEW_EMAIL, _johnUser.Email);
        BCrypt.Net.BCrypt.Verify(NEW_PASSWORD, _johnUser.PasswordHash);
    }

    [TestMethod]
    public async Task UpdateUserAsync_NonAdmin_SameUser_Ok()
    {
        // -- Arrange --
        _mapperMock.Setup(m => m.Map<UserGetDto>(It.IsAny<User>()))
                    .Returns(new UserGetDto
                    {
                        FirstName = NEW_FIRST_NAME,
                        LastName = NEW_LAST_NAME,
                        Email = NEW_EMAIL,
                        JoinDateUtc = _johnUser.JoinDateUtc
                    });
        var userService = new UserService(
            _dbContextMock.Object,
            _jwtServiceMock.Object,
            _configMock.Object,
            _mapperMock.Object);

        // -- Act --
        var returnedDto = await userService.UpdateUserAsync(
            JOHN_USER_GUID,
            JOHN_USER_GUID,
            new UserUpdateDto
            {
                FirstName = NEW_FIRST_NAME,
                LastName = NEW_LAST_NAME,
                Email = NEW_EMAIL,
                Password = NEW_PASSWORD
            }
        );

        // -- Assert --
        Assert.IsNotNull(returnedDto);

        // Check that the returned user is as expected
        _assertUsersSame(returnedDto, NEW_FIRST_NAME, NEW_LAST_NAME, NEW_EMAIL);

        // Check that the saved user is as expected
        _dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Once);
        Assert.AreEqual(NEW_FIRST_NAME, _johnUser.FirstName);
        Assert.AreEqual(NEW_LAST_NAME, _johnUser.LastName);
        Assert.AreEqual(NEW_EMAIL, _johnUser.Email);
        BCrypt.Net.BCrypt.Verify(NEW_PASSWORD, _johnUser.PasswordHash);
    }

    [TestMethod]
    [ExpectedException(typeof(ForbiddenException))]
    public async Task UpdateUserAsync_NonAdmin_AnotherUser_ThrowForbidden()
    {
        // -- Arrange --
        var userService = new UserService(
            _dbContextMock.Object,
            _jwtServiceMock.Object,
            _configMock.Object,
            _mapperMock.Object);

        var userUpdateDto = new UserUpdateDto
            {
                FirstName = NEW_FIRST_NAME,
                LastName = NEW_LAST_NAME,
                Email = NEW_EMAIL,
                Password = NEW_PASSWORD
            };

        // -- Act --
        var returnedDto = await userService.UpdateUserAsync(
            JOHN_USER_GUID,
            JANE_USER_GUID,
            userUpdateDto
        );
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidDataException))]
    public async Task UpdateUserAsync_NonAdmin_SameUser_NullUpdateObject_ThrowInvalidData()
    {
        // -- Arrange --
        var userService = new UserService(
            _dbContextMock.Object,
            _jwtServiceMock.Object,
            _configMock.Object,
            _mapperMock.Object);

        // -- Act --
        var returnedDto = await userService.UpdateUserAsync(
            JOHN_USER_GUID,
            JOHN_USER_GUID,
            null
        );
    }

    [TestMethod]
    public async Task UpdateUserAsync_NonAdmin_SameUser_EmptyUpdateObject_Ok()
    {
        // -- Arrange --
        var userService = new UserService(
            _dbContextMock.Object,
            _jwtServiceMock.Object,
            _configMock.Object,
            _mapperMock.Object);
        var userUpdateDto = new UserUpdateDto();

        // -- Act --
        var returnedDto = await userService.UpdateUserAsync(
            JOHN_USER_GUID,
            JOHN_USER_GUID,
            userUpdateDto
        );

        // -- Assert --
        Assert.IsNull(returnedDto);
        // Check that the user in DB didn't change
        _dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Never);
        Assert.AreEqual(JOHN_FIRST_NAME, _johnUser.FirstName);
        Assert.AreEqual(COMMON_LAST_NAME, _johnUser.LastName);
        Assert.AreEqual(JOHN_EMAIL, _johnUser.Email);
        BCrypt.Net.BCrypt.Verify(COMMON_OLD_PASSWORD, _johnUser.PasswordHash);
    }

    [TestMethod]
    public async Task UpdateUserAsync_Admin_NonexistentUser_ReturnNull()
    {
        // -- Arrange --
        var userService = new UserService(
            _dbContextMock.Object,
            _jwtServiceMock.Object,
            _configMock.Object,
            _mapperMock.Object);
        var userUpdateDto = new UserUpdateDto()
            {
                FirstName = "NewPerson"
            };

        // -- Act --
        var returnedDto = await userService.UpdateUserAsync(
            JANE_USER_GUID,
            Guid.NewGuid(),
            userUpdateDto
        );

        // -- Assert --
        Assert.IsNull(returnedDto);
        _dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Never);
    }
    
    [TestMethod]
    public async Task UpdateUserAsync_Admin_DeletedUser_ReturnNull()
    {
        // -- Arrange --
        var userService = new UserService(
            _dbContextMock.Object,
            _jwtServiceMock.Object,
            _configMock.Object,
            _mapperMock.Object);
        var userUpdateDto = new UserUpdateDto()
            {
                FirstName = "Juju"
            };

        // -- Act --
        var returnedDto = await userService.UpdateUserAsync(
            JANE_USER_GUID,
            JUDY_USER_GUID,
            userUpdateDto
        );

        // -- Assert --
        Assert.IsNull(returnedDto);
        _dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Never);
        Assert.AreEqual(JUDY_FIRST_NAME, _judyUser.FirstName);
        Assert.AreEqual(COMMON_LAST_NAME, _judyUser.LastName);
        Assert.AreEqual(JUDY_EMAIL, _judyUser.Email);
        BCrypt.Net.BCrypt.Verify(COMMON_OLD_PASSWORD, _judyUser.PasswordHash);
    }
    #endregion

    #region DeleteUserAsync
    // TODO check deletes cascade for activities and sessions
    [TestMethod]
    public async Task DeleteUserAsync_Admin_AnotherUser_Ok()
    {
        // -- Arrange --
        var userService = new UserService(
            _dbContextMock.Object,
            _jwtServiceMock.Object,
            _configMock.Object,
            _mapperMock.Object);

        // -- Act --
        var isSuccess = await userService.DeleteUserAsync(JANE_USER_GUID, JOHN_USER_GUID);

        // -- Assert --
        Assert.IsTrue(isSuccess);
        Assert.IsNotNull(_johnUser.DeletedDateUtc);
        _dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Once);
        // Check that the dates are equal within a minute
        Assert.IsTrue(DatesEqualWithinSeconds((DateTime) _johnUser.DeletedDateUtc, DateTime.UtcNow));
    }

    [TestMethod]
    public async Task DeleteUserAsync_Admin_AnotherNonExistentUser_ReturnFalse()
    {
        // -- Arrange --
        var userService = new UserService(
            _dbContextMock.Object,
            _jwtServiceMock.Object,
            _configMock.Object,
            _mapperMock.Object);

        // -- Act --
        var isSuccess = await userService.DeleteUserAsync(JANE_USER_GUID, Guid.NewGuid());

        // -- Assert --
        _dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Never);
        Assert.IsFalse(isSuccess);
    }

    [TestMethod]
    public async Task DeleteUserAsync_Admin_AnotherDeletedUser_ReturnFalse()
    {
        // -- Arrange --
        var userService = new UserService(
            _dbContextMock.Object,
            _jwtServiceMock.Object,
            _configMock.Object,
            _mapperMock.Object);

        // -- Act --
        var isSuccess = await userService.DeleteUserAsync(JANE_USER_GUID, JUDY_USER_GUID);

        // -- Assert --
        _dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Never);
        Assert.IsFalse(isSuccess);
    }

    [TestMethod]
    public async Task DeleteUserAsync_NonAdmin_SameUser_Ok()
    {
        // -- Arrange --
        var userService = new UserService(
            _dbContextMock.Object,
            _jwtServiceMock.Object,
            _configMock.Object,
            _mapperMock.Object);

        // -- Act --
        var isSuccess = await userService.DeleteUserAsync(JOHN_USER_GUID, JOHN_USER_GUID);

        // -- Assert --
        Assert.IsTrue(isSuccess);
        Assert.IsNotNull(_johnUser.DeletedDateUtc);
        _dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Once);
        // Check that the dates are equal within a minute
        Assert.IsTrue(DatesEqualWithinSeconds((DateTime) _johnUser.DeletedDateUtc, DateTime.UtcNow));
    }

    [TestMethod]
    [ExpectedException(typeof(ForbiddenException))]
    public async Task DeleteUserAsync_NonAdmin_AnotherUser_ThrowForbidden()
    {
        // -- Arrange --
        var userService = new UserService(
            _dbContextMock.Object,
            _jwtServiceMock.Object,
            _configMock.Object,
            _mapperMock.Object);

        // -- Act --
        var isSuccess = await userService.DeleteUserAsync(JOHN_USER_GUID, JANE_USER_GUID);
    }
    #endregion

    private void _assertUsersSame(UserGetDto user, string firstName, string lastName, string email)
    {
        Assert.IsTrue(user != null);
        Assert.AreEqual(firstName, user.FirstName);
        Assert.AreEqual(lastName, user.LastName);
        Assert.AreEqual(email, user.Email);

        // Check that the dates are equal within a minute
        Assert.IsTrue(DatesEqualWithinSeconds((DateTime) user.JoinDateUtc, DateTime.UtcNow));
    }
}