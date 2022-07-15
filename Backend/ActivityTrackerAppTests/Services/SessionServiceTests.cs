using ActivityTrackerApp.Database;
using ActivityTrackerApp.Entities;
using ActivityTrackerApp.Services;
using static ActivityTrackerAppTests.Fixtures.TestFixtures;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using ActivityTrackerApp.Dtos;
using MockQueryable.Moq;

namespace ActivityTrackerAppTests;

// TODO: Need to finish these tests
[TestClass]
public class SessionServiceTests
{
    private static List<User> _usersData;

    private static Activity _panicAct;
    private static Activity _baseballAct;

    private static Activity _gameDevAct;
    private static Session _gameDevSession1;
    private static Session _gameDevSession2;

    private static Activity _pianoAct;
    private static Session _pianoSession1;
    private static Session _pianoSession2;

    private static Activity _mcatAct;
    private static Activity _sleepingAct;
    private static List<Activity> _janesActs;
    private static List<Activity> _johnsActs;
    private static List<Activity> _judysActs;
    private static List<Activity> _allActs;

    private static Mock<DbSet<User>> _usersDbSetMock;
    private static Mock<DbSet<Activity>> _activitiesDbSetMock;
    private static Mock<IDataContext> _dbContextMock;
    private static Mock<IUserService> _userServiceMock;
    private static Mock<ISessionService> _sessionServiceMock;
    private static Mock<IMapper> _mapperMock;

    // Called before all tests
    [ClassInitialize()]
    public static void InitializeClass(TestContext context)
    {
        // Init users here since they won't change through each activity test
        _usersData = new List<User> { GenerateJaneUser(), GenerateJohnUser(), GenerateJudyUser() };
    }

    // Called before each test
    [TestInitialize]
    public void InitializeTests()
    {
        _panicAct = GeneratePanicActivity();
        _janesActs = new List<Activity> { _panicAct };

        _gameDevAct = GenerateGameDevActivity();
        _gameDevSession1 = GenerateGameDevSession1();
        _gameDevSession2 = GenerateGameDevSession2();
        _gameDevAct.Sessions = new List<Session> { _gameDevSession1, _gameDevSession2 };
        _pianoAct = GeneratePianoActivity();
        _pianoSession1 = GeneratePianoSession1();
        _pianoSession2 = GeneratePianoSession2();
        _pianoAct.Sessions = new List<Session> { _pianoSession1, _pianoSession2 };
        _mcatAct = GenerateMcatActivity();
        _sleepingAct = GenerateSleepingActivity();
        _johnsActs = new List<Activity> { _gameDevAct, _pianoAct, _mcatAct, _sleepingAct };

        _baseballAct = GenerateBaseballActivity();
        _judysActs = new List<Activity> { _baseballAct };

        _allActs = new List<Activity> { _panicAct, _gameDevAct, _pianoAct, _mcatAct, _sleepingAct, _baseballAct };

        // Set up mock objects
        _usersDbSetMock = _usersData.AsQueryable().BuildMockDbSet();
        _activitiesDbSetMock = _allActs.AsQueryable().BuildMockDbSet();
        _dbContextMock = new Mock<IDataContext>();
        _dbContextMock.Setup(x => x.Users)
                .Returns(_usersDbSetMock.Object);
        _dbContextMock.Setup(x => x.Activities)
                .Returns(_activitiesDbSetMock.Object);
        _userServiceMock = new Mock<IUserService>();
        _sessionServiceMock = new Mock<ISessionService>();
        _mapperMock = new Mock<IMapper>();
    }


    #region GetAllSessionsAsync
    [TestMethod]
    public async Task GetAllSessionsByActivityIdAsync_Admin_AnotherUser_Ok()
    {
        // -- Arrange --
        _userServiceMock.Setup(m => m.IsAdmin(JANE_USER_GUID))
                            .Returns(Task.FromResult(true));

        var sessionService = new SessionService(
            _dbContextMock.Object,
            _userServiceMock.Object,
            _mapperMock.Object);

        // -- Act --
        var sessions = await sessionService.GetAllSessionsByActivityIdAsync(JANE_USER_GUID);

        // -- Assert --
        Assert.IsNotNull(sessions);
        Assert.AreEqual(sessions.Count(), 3);
        var sessionsList = sessions.ToList();

        throw new NotImplementedException();
    }

    [TestMethod]
    public async Task GetAllSessionsByActivityIdAsync_NonAdmin_OwnSession_Ok()
    {
        // -- Arrange --
        _userServiceMock.Setup(m => m.IsAdmin(JANE_USER_GUID))
                            .Returns(Task.FromResult(true));

        var sessionService = new SessionService(
            _dbContextMock.Object,
            _userServiceMock.Object,
            _mapperMock.Object);

        // -- Act --
        var sessions = await sessionService.GetAllSessionsByActivityIdAsync(JOHN_USER_GUID);

        // -- Assert --
        Assert.IsNotNull(sessions);
        Assert.AreEqual(sessions.Count(), 3);
        var activitiesList = sessions.ToList();

        throw new NotImplementedException();
    }
    #endregion

    #region GetSessionAsync
    #endregion GetSessionAsync

    #region CreateSessionAsync
    #endregion CreateSessionAsync
    
    #region UpdateSessionAsync
    #endregion UpdateSessionAsync
    
    #region DeleteSessionAsync
    #endregion DeleteSessionAsync
}
