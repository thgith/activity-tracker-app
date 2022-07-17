using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using ActivityTrackerApp.Constants;
using ActivityTrackerApp.Database;
using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Entities;
using ActivityTrackerApp.Exceptions;
using ActivityTrackerApp.Services;

using AutoMapper;

using MockQueryable.Moq;

using Moq;

using static ActivityTrackerAppTests.Fixtures.TestFixtures;
using static ActivityTrackerAppTests.Helpers.TestHelpers;

namespace ActivityTrackerAppTests;

// TODO to check cascade delete and password reset
// NOTE: Prob should add more checks to check side effects (call count, etc),
//       but this is fine for now
[TestClass]
public class UserServiceTests
{
    private static User _janeUser;
    private static User _johnUser;
    private static User _judyUser;
    private static List<User> _usersData;
    private static Mock<DbSet<Activity>> _activitiesDbSetMock;
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
        _setUpUsers();
        _setUpMocks();
    }

    private void _setUpUsers()
    {
        _janeUser = GenerateJaneUser();
        _johnUser = GenerateJohnUser();
        // Deleted user
        _judyUser = GenerateJudyUser();
        _usersData = new List<User> { _janeUser, _johnUser, _judyUser };
    }

    private void _setUpMocks()
    {
        _usersDbSetMock = _usersData.AsQueryable().BuildMockDbSet();
        _jwtServiceMock = new Mock<IJwtService>();
        _configMock = new Mock<IConfiguration>();

        _setUpDbMock();
        _setUpMapperMock();

        void _setUpDbMock()
        {
            _dbContextMock = new Mock<IDataContext>();
            _dbContextMock.Setup(x => x.Users)
                    .Returns(_usersDbSetMock.Object);

            // TODO should test cascade delete
            _activitiesDbSetMock = (new List<Activity>()).AsQueryable().BuildMockDbSet();
            _dbContextMock.Setup(x => x.Activities)
                    .Returns(_activitiesDbSetMock.Object);
        }

        void _setUpMapperMock()
        {
            _mapperMock = new Mock<IMapper>();

            _mapperMock.Setup(x => x.Map<UserGetDto>(It.Is<User>(x => x == null)))
                .Returns<UserGetDto>(null);

            _mapperMock.Setup(x => x.Map<UserGetDto>(It.Is<User>(x => x != null)))
                .Returns((User user) =>
                    new UserGetDto
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        JoinDateUtc = user.JoinDateUtc
                    }
                );

            _mapperMock.Setup(m => m.Map<User>(It.Is<UserRegisterDto>(x => x == null)))
                        .Returns<User>(null);

            _mapperMock.Setup(m => m.Map<User>(It.Is<UserRegisterDto>(x => x != null)))
                        .Returns((UserRegisterDto userDto) =>
                            new User
                            {
                                Id = Guid.Empty,
                                FirstName = userDto.FirstName,
                                LastName = userDto.LastName,
                                Email = userDto.Email,
                                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
                                // Technically we do more with this date, but this is close enough
                                JoinDateUtc = DateTime.UtcNow,
                                DeletedDateUtc = null,
                                Role = Roles.MEMBER
                            }
                        );
        }
    }

    #region GetAllUsersAsync
    [TestMethod]
    [TestCategory(nameof(UserService.GetAllUsersAsync))]
    public async Task GetAllUsersAsync_Admin_Ok()
    {
        // -- Arrange --
        var userService = _createUserService();

        // -- Act --
        var users = await userService.GetAllUsersAsync(JANE_USER_GUID);

        // -- Assert --
        Assert.IsNotNull(users);
        Assert.AreEqual(users.Count(), 2);
        var usersList = users.ToList();

        // This list should be ordered by join date, so the users should be in this order
        // Didn't get deleted user
        _assertUsersSame(usersList[0], JANE_USER_GUID, JANE_FIRST_NAME, COMMON_LAST_NAME, JANE_EMAIL);
        _assertUsersSame(usersList[1], JOHN_USER_GUID, JOHN_FIRST_NAME, COMMON_LAST_NAME, JOHN_EMAIL);

        _dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Never);
    }

    [TestMethod]
    [TestCategory(nameof(UserService.GetAllUsersAsync))]
    [TestCategory(nameof(ForbiddenException))]
    [ExpectedException(typeof(ForbiddenException))]
    public async Task GetAllUsersAsync_NotAdmin_ThrowForbidden()
    {
        // -- Arrange --
        var userService = _createUserService();

        // -- Act --
        var users = await userService.GetAllUsersAsync(JOHN_USER_GUID);
    }

    [TestMethod]
    [TestCategory(nameof(UserService.GetUserAsync))]
    public async Task GetUserAsync_Admin_AnotherUser_Ok()
    {
        // -- Arrange --
        var userService = _createUserService();

        // -- Act --
        // Try to get John's info as admin Jane
        var user = await userService.GetUserAsync(JANE_USER_GUID, JOHN_USER_GUID);

        // -- Assert --
        Assert.IsNotNull(user);
        _assertUsersSame(user, JOHN_USER_GUID, JOHN_FIRST_NAME, COMMON_LAST_NAME, JOHN_EMAIL);

        _dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Never);
    }
    #endregion

    #region GetUserAsync
    [TestMethod]
    [TestCategory(nameof(UserService.GetUserAsync))]
    public async Task GetUserAsync_Admin_AnotherNonExistentUser_ReturnNull()
    {
        // -- Arrange --
        var userService = _createUserService();

        // -- Act --
        // Try to get nonexistent as admin Jane
        var user = await userService.GetUserAsync(JANE_USER_GUID, Guid.NewGuid());

        // -- Assert --
        Assert.IsNull(user);

        _dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Never);
    }

    [TestMethod]
    [TestCategory(nameof(UserService.GetUserAsync))]
    public async Task GetUserAsync_Admin_AnotherDeletedUser_ReturnNull()
    {
        // -- Arrange --
        var userService = _createUserService();

        // -- Act --
        // Try to get deleted user Judy's info as admin Jane
        var user = await userService.GetUserAsync(JANE_USER_GUID, JUDY_USER_GUID);

        // -- Assert --
        Assert.IsNull(user);

        _dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Never);
    }

    [TestMethod]
    [TestCategory(nameof(UserService.GetUserAsync))]
    public async Task GetUserAsync_NonAdmin_SameUser_Ok()
    {
        // -- Arrange --
        var userService = _createUserService();

        // -- Act --
        // Try to get John's info as member John
        var user = await userService.GetUserAsync(JOHN_USER_GUID, JOHN_USER_GUID);

        // -- Assert --
        Assert.IsNotNull(user);
        _assertUsersSame(user, JOHN_USER_GUID, JOHN_FIRST_NAME, COMMON_LAST_NAME, JOHN_EMAIL);

        _dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Never);
    }

    [TestMethod]
    [TestCategory(nameof(UserService.GetUserAsync))]
    [TestCategory(nameof(ForbiddenException))]
    [ExpectedException(typeof(ForbiddenException))]
    public async Task GetUserAsync_NonAdmin_AnotherUser_ThrowForbidden()
    {
        // -- Arrange --
        var userService = _createUserService();

        // -- Act --
        // Try to get Jane's info as member John
        var user = await userService.GetUserAsync(JOHN_USER_GUID, JANE_USER_GUID);

        _dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Never);
    }
    #endregion

    #region RegisterUserAsync
    // The model annotations and some ctrl checks handle a lot of the err
    // checking, so for now this is fine.
    [TestMethod]
    [TestCategory(nameof(UserService.RegisterUserAsync))]
    public async Task RegisterUserAsync_Ok()
    {
        // -- Arrange --
        var dummyToken = "dummytoken";
        var newUserGuid = Guid.NewGuid();
        _jwtServiceMock.Setup(m => m.GenerateJwtToken(It.IsAny<User>(), 300))
                        .Returns(dummyToken);

        var userService = _createUserService();

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

        // Replicate the save setting a new GUID for the new user
        _dbContextMock.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                        .Callback((CancellationToken _) => { _usersData.Last().Id = newUserGuid; });

        // -- Act --
        var returnedEntityWithToken = await userService.RegisterUserAsync(registerUserDto);
        var returnedEntity = ((EntityWithToken<UserGetDto>)returnedEntityWithToken).Entity;
        var returnedToken = ((EntityWithToken<UserGetDto>)returnedEntityWithToken).Token;

        // -- Assert --
        Assert.IsNotNull(returnedEntityWithToken);
        Assert.IsNotNull(returnedToken);

        // Check that the returned object is as expected
        _assertUsersSame(returnedEntity, newUserGuid, LILA_FIRST_NAME, COMMON_LAST_NAME, LILA_EMAIL);
        Assert.AreEqual(dummyToken, returnedToken);

        _usersDbSetMock.Verify(m => m.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Once);

        // This saved user should have been added during the AddAsync callback
        Assert.AreEqual(_usersData.Count(), 4);
        var newUserLila = _usersData.Last();

        // Check that the data was saved to the DB correctly
        Assert.AreNotEqual(Guid.Empty, newUserLila.Id);
        Assert.AreEqual(LILA_FIRST_NAME, newUserLila.FirstName);
        Assert.AreEqual(COMMON_LAST_NAME, newUserLila.LastName);
        Assert.AreEqual(LILA_EMAIL, newUserLila.Email);
        BCrypt.Net.BCrypt.Verify(COMMON_OLD_PASSWORD, newUserLila.PasswordHash);
        Assert.IsTrue(DatesEqualWithinSeconds((DateTime)newUserLila.JoinDateUtc, DateTime.UtcNow));
        Assert.IsNull(newUserLila.DeletedDateUtc);
        Assert.AreEqual(Roles.MEMBER, newUserLila.Role);
    }

    [TestMethod]
    [TestCategory(nameof(UserService.RegisterUserAsync))]
    [TestCategory(nameof(InvalidDataException))]
    [ExpectedException(typeof(InvalidDataException))]
    public async Task RegisterUserAsync_NullObject_ThrowInvalidData()
    {
        // -- Arrange --
        var userService = _createUserService();

        // -- Act --
        var returnedEntityWithToken = await userService.RegisterUserAsync(null);
    }
    #endregion

    #region UpdateUserAsync
    [TestMethod]
    [TestCategory(nameof(UserService.UpdateUserAsync))]
    public async Task UpdateUserAsync_Admin_AnotherUser_Ok()
    {
        // -- Arrange --
        var userService = _createUserService();

        // -- Act --
        var user = await userService.UpdateUserAsync(
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
        _assertUpdateOk(user);
    }


    [TestMethod]
    [TestCategory(nameof(UserService.UpdateUserAsync))]
    public async Task UpdateUserAsync_Admin_NonExistentUser_ReturnNull()
    {
        // -- Arrange --
        var userService = _createUserService();

        var userUpdateDto = new UserUpdateDto()
        {
            FirstName = "DoesNotExist"
        };

        // -- Act --
        var user = await userService.UpdateUserAsync(
            JANE_USER_GUID,
            Guid.NewGuid(),
            userUpdateDto
        );

        // -- Assert --
        _assertUpdateReturnNull(user);
    }

    [TestMethod]
    [TestCategory(nameof(UserService.UpdateUserAsync))]
    public async Task UpdateUserAsync_Admin_DeletedUser_ReturnNull()
    {
        // -- Arrange --
        var userService = _createUserService();

        var userUpdateDto = new UserUpdateDto()
        {
            FirstName = "Juju"
        };

        // -- Act --
        var user = await userService.UpdateUserAsync(
            JANE_USER_GUID,
            JUDY_USER_GUID,
            userUpdateDto
        );

        // -- Assert --
        _assertUpdateReturnNull(user);
    }

    [TestMethod]
    [TestCategory(nameof(UserService.UpdateUserAsync))]
    public async Task UpdateUserAsync_NonAdmin_SameUser_Ok()
    {
        // -- Arrange --
        var userService = _createUserService();

        // -- Act --
        var user = await userService.UpdateUserAsync(
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
        _assertUpdateOk(user);
    }

    [TestMethod]
    [TestCategory(nameof(UserService.UpdateUserAsync))]
    [TestCategory(nameof(ForbiddenException))]
    [ExpectedException(typeof(ForbiddenException))]
    public async Task UpdateUserAsync_NonAdmin_AnotherUser_ThrowForbidden()
    {
        // -- Arrange --
        var userService = _createUserService();

        // -- Act --
        var returnedDto = await userService.UpdateUserAsync(
            JOHN_USER_GUID,
            JANE_USER_GUID,
            new UserUpdateDto()
        );
    }

    [TestMethod]
    [TestCategory(nameof(UserService.UpdateUserAsync))]
    [TestCategory(nameof(InvalidDataException))]
    [ExpectedException(typeof(InvalidDataException))]
    public async Task UpdateUserAsync_NonAdmin_SameUser_NullUpdateObject_ThrowInvalidData()
    {
        // -- Arrange --
        var userService = _createUserService();

        // -- Act --
        var returnedDto = await userService.UpdateUserAsync(
            JOHN_USER_GUID,
            JOHN_USER_GUID,
            null
        );
    }

    [TestMethod]
    [TestCategory(nameof(UserService.UpdateUserAsync))]
    [ExpectedException(typeof(InvalidDataException))]
    public async Task UpdateUserAsync_NonAdmin_SameUser_TakenEmail_ThrowInvalidData()
    {
        // -- Arrange --
        var userService = _createUserService();

        // -- Act --
        var user = await userService.UpdateUserAsync(
            JOHN_USER_GUID,
            JOHN_USER_GUID,
            new UserUpdateDto
            {
                FirstName = NEW_FIRST_NAME,
                LastName = NEW_LAST_NAME,
                Email = JANE_EMAIL,
                Password = NEW_PASSWORD
            }
        );
    }

    [TestMethod]
    [TestCategory(nameof(UserService.UpdateUserAsync))]
    public async Task UpdateUserAsync_NonAdmin_SameUser_EmptyUpdateObject_Ok()
    {
        // -- Arrange --
        var userService = _createUserService();

        // -- Act --
        var returnedDto = await userService.UpdateUserAsync(
            JOHN_USER_GUID,
            JOHN_USER_GUID,
            new UserUpdateDto()
        );

        // -- Assert --
        Assert.IsNotNull(returnedDto);
        _assertUsersSame(returnedDto, JOHN_USER_GUID, JOHN_FIRST_NAME, COMMON_LAST_NAME, JOHN_EMAIL);

        // Check that the user in DB didn't change
        _dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Never);
        Assert.AreEqual(JOHN_FIRST_NAME, _johnUser.FirstName);
        Assert.AreEqual(COMMON_LAST_NAME, _johnUser.LastName);
        Assert.AreEqual(JOHN_EMAIL, _johnUser.Email);
        BCrypt.Net.BCrypt.Verify(COMMON_OLD_PASSWORD, _johnUser.PasswordHash);
    }
    #endregion

    #region DeleteUserAsync
    // TODO check deletes cascade for activities and sessions
    [TestMethod]
    [TestCategory(nameof(UserService.DeleteUserAsync))]
    public async Task DeleteUserAsync_Admin_AnotherUser_Ok()
    {
        // -- Arrange --
        var userService = _createUserService();

        // -- Act --
        var isSuccess = await userService.DeleteUserAsync(JANE_USER_GUID, JOHN_USER_GUID);

        // -- Assert --
        _assertDeleteOk(isSuccess);
    }

    [TestMethod]
    [TestCategory(nameof(UserService.DeleteUserAsync))]
    public async Task DeleteUserAsync_Admin_AnotherNonExistentUser_ReturnFalse()
    {
        // -- Arrange --
        var userService = _createUserService();

        // -- Act --
        var isSuccess = await userService.DeleteUserAsync(JANE_USER_GUID, Guid.NewGuid());

        // -- Assert --
        _assertDeleteReturnFalse(isSuccess);
    }

    [TestMethod]
    [TestCategory(nameof(UserService.DeleteUserAsync))]
    public async Task DeleteUserAsync_Admin_AnotherDeletedUser_ReturnFalse()
    {
        // -- Arrange --
        var userService = _createUserService();

        // -- Act --
        var isSuccess = await userService.DeleteUserAsync(JANE_USER_GUID, JUDY_USER_GUID);

        // -- Assert --
        _assertDeleteReturnFalse(isSuccess);
    }

    [TestMethod]
    [TestCategory(nameof(UserService.DeleteUserAsync))]
    public async Task DeleteUserAsync_NonAdmin_SameUser_Ok()
    {
        // -- Arrange --
        var userService = _createUserService();

        // -- Act --
        var isSuccess = await userService.DeleteUserAsync(JOHN_USER_GUID, JOHN_USER_GUID);

        // -- Assert --
        _assertDeleteOk(isSuccess);
    }

    [TestMethod]
    [TestCategory(nameof(UserService.DeleteUserAsync))]
    [TestCategory(nameof(ForbiddenException))]
    [ExpectedException(typeof(ForbiddenException))]
    public async Task DeleteUserAsync_NonAdmin_AnotherUser_ThrowForbidden()
    {
        // -- Arrange --
        var userService = _createUserService();

        // -- Act --
        var isSuccess = await userService.DeleteUserAsync(JOHN_USER_GUID, JANE_USER_GUID);
    }
    #endregion

    private UserService _createUserService()
    {
        return new UserService(
            _dbContextMock.Object,
            _jwtServiceMock.Object,
            _configMock.Object,
            _mapperMock.Object);
    }

    private void _assertUsersSame(UserGetDto returnedUser, Guid id, string firstName, string lastName, string email)
    {
        Assert.IsTrue(returnedUser != null);
        Assert.AreEqual(id, returnedUser.Id);
        Assert.AreEqual(firstName, returnedUser.FirstName);
        Assert.AreEqual(lastName, returnedUser.LastName);
        Assert.AreEqual(email, returnedUser.Email);

        // Check that the dates are equal within a threshold
        Assert.IsTrue(DatesEqualWithinSeconds((DateTime)returnedUser.JoinDateUtc, DateTime.UtcNow));
    }

    private void _assertUpdateOk(UserGetDto returnedUser)
    {
        Assert.IsNotNull(returnedUser);

        // Check that the returned user is as expected
        _assertUsersSame(returnedUser, JOHN_USER_GUID, NEW_FIRST_NAME, NEW_LAST_NAME, NEW_EMAIL);

        // Check that the DB is as expected
        _dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Once);
        Assert.AreEqual(JOHN_USER_GUID, _johnUser.Id);
        Assert.AreEqual(NEW_FIRST_NAME, _johnUser.FirstName);
        Assert.AreEqual(NEW_LAST_NAME, _johnUser.LastName);
        Assert.AreEqual(NEW_EMAIL, _johnUser.Email);
        BCrypt.Net.BCrypt.Verify(NEW_PASSWORD, _johnUser.PasswordHash);
    }

    private void _assertUpdateReturnNull(UserGetDto returnedUser)
    {
        Assert.IsNull(returnedUser);
        _dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Never);
    }

    private void _assertDeleteOk(bool isSuccess)
    {
        Assert.IsTrue(isSuccess);
        Assert.IsNotNull(_johnUser.DeletedDateUtc);
        _dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Once);
        // Check that the dates are equal within a threshold
        Assert.IsTrue(DatesEqualWithinSeconds((DateTime)_johnUser.DeletedDateUtc, DateTime.UtcNow));
    }

    private void _assertDeleteReturnFalse(bool isSuccess)
    {
        _dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Never);
        Assert.IsFalse(isSuccess);
    }
}