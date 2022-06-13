using ActivityTrackerApp.Constants;
using ActivityTrackerApp.Database;
using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Entities;
using ActivityTrackerApp.Exceptions;
using ActivityTrackerApp.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MockQueryable.Moq;
using Moq;

namespace ActivityTrackerAppTests;

[TestClass]
public class UserServiceTests
{
    public static Guid JANE_USER_GUID;
    public static Guid JOHN_USER_GUID;
    public static Guid JUDY_USER_GUID;
    public static readonly string JANE_FIRST_NAME = "Jane";
    public static readonly string JOHN_FIRST_NAME = "John";
    public static readonly string JUDY_FIRST_NAME = "Judy";
    public static readonly string COMMON_LAST_NAME = "Doe";
    public static readonly string JANE_EMAIL = "janedoe@test.com";
    public static readonly string JOHN_EMAIL = "johndoe@test.com";
    public static DateTime JANE_JOIN_DATE_UTC;
    public static DateTime JOHN_JOIN_DATE_UTC;

    private static User _janeUser;
    private static User _joeUser;
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
        JANE_USER_GUID = Guid.NewGuid();
        JANE_JOIN_DATE_UTC = DateTime.UtcNow;

        JOHN_USER_GUID = Guid.NewGuid();
        JOHN_JOIN_DATE_UTC = DateTime.UtcNow.AddSeconds(1);

        JUDY_USER_GUID = Guid.NewGuid();
    }

    // Called before each test
    [TestInitialize]
    public void InitializeTests()
    {
        _janeUser = new User
        {
            Id = JANE_USER_GUID,
            FirstName = JANE_FIRST_NAME,
            LastName = COMMON_LAST_NAME,
            Email = JANE_EMAIL,
            PasswordHash = "asdf",
            JoinDateUtc = JANE_JOIN_DATE_UTC,
            DeletedDateUtc = null,
            Role = Roles.ADMIN
        };
        _joeUser = new User
        {
            Id = JOHN_USER_GUID,
            FirstName = JOHN_FIRST_NAME,
            LastName = COMMON_LAST_NAME,
            Email = JOHN_EMAIL,
            PasswordHash = "asdf",
            // To make this user's join date later than the first user's
            JoinDateUtc = JOHN_JOIN_DATE_UTC,
            DeletedDateUtc = null,
            Role = Roles.MEMBER
        };
        // Deleted user
        _judyUser = new User
            {
                Id = JUDY_USER_GUID,
                FirstName = JUDY_FIRST_NAME,
                LastName = COMMON_LAST_NAME,
                Email = "judydoe@test.com",
                PasswordHash = "asdf",
                JoinDateUtc = DateTime.UtcNow.AddSeconds(2),
                DeletedDateUtc = DateTime.UtcNow,
                Role = Roles.MEMBER
            };
        _usersData = new List<User> { _janeUser, _joeUser, _judyUser };

        // Set up mock objects
        _usersDbSetMock = _usersData.AsQueryable().BuildMockDbSet();
        _dbContextMock = new Mock<IDataContext>();
        _jwtServiceMock = new Mock<IJwtService>();
        _configMock = new Mock<IConfiguration>();
        _mapperMock = new Mock<IMapper>();
    }

    [TestMethod]
    public async Task GetAllUsersAsync_Admin_Ok()
    {
        // -- Arrange --
        _dbContextMock.Setup(x => x.Users)
                        .Returns(_usersDbSetMock.Object);
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
        _mapperMock.Setup(x => x.Map<UserGetDto>(_joeUser))
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
        _dbContextMock.Setup(x => x.Users)
                        .Returns(_usersDbSetMock.Object);

        var userService = new UserService(
            _dbContextMock.Object,
            _jwtServiceMock.Object,
            _configMock.Object,
            _mapperMock.Object);

        // -- Act --
        var users = await userService.GetAllUsersAsync(JOHN_USER_GUID);
    }

    private void _assertUsersSame(UserGetDto user, string firstName, string lastName, string email)
    {
        Assert.IsTrue(user != null);
        Assert.AreEqual(firstName, user.FirstName);
        Assert.AreEqual(lastName, user.LastName);
        Assert.AreEqual(email, user.Email);

        // Check that the dates are equal within a minute
        TimeSpan timeSpan = DateTime.UtcNow.Subtract((DateTime) user.JoinDateUtc);
        Assert.IsTrue(timeSpan.TotalMinutes < 1);
    }
}

