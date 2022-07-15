using ActivityTrackerApp.Database;
using ActivityTrackerApp.Entities;
using ActivityTrackerApp.Services;
using static ActivityTrackerAppTests.Fixtures.TestFixtures;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using ActivityTrackerApp.Dtos;
using MockQueryable.Moq;
using ActivityTrackerApp.Exceptions;

namespace ActivityTrackerAppTests;

// TODO: Add sessions and test
// TODO test 
//  startDateUtc null
//  startDateUtc > dueDateUtc
//  startDateUtc > completedDateUtc
//  etc other paths
// NOTE: Prob should add more checks to check side effects (call count, etc),
//       but this is fine for now
[TestClass]
public class ActivityServiceTests
{
    private static List<User> _usersData;

    private static Activity _panicAct;
    private static Activity _baseballAct;
    private static Activity _gameDevAct;
    private static Session _gameDevSesh1;
    private static Session _gameDevSesh2;
    private static Activity _pianoAct;
    private static Session _pianoSesh1;
    private static Session _pianoSesh2;
    private static Activity _mcatAct;
    private static Activity _sleepingAct;

    private static List<Activity> _janesActs;
    private static List<Activity> _johnsActs;
    private static List<Activity> _judysActs;

    private static List<Session> _allSessions;
    private static List<Activity> _allActs;

    private static Mock<DbSet<User>> _usersDbSetMock;
    private static Mock<DbSet<Activity>> _activitiesDbSetMock;
    private static Mock<DbSet<Session>> _sessionsDbSetMock;
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
        _setUpActivitiesAndSessions();

        // Set up mock objects
        _setUpDbMocks();
        _setUpMapperMocks();
        _setUpServiceMocks();
    }

    private void _setUpActivitiesAndSessions()
    {
        // Jane's acts
        _panicAct = GeneratePanicActivity();
        _janesActs = new List<Activity> { _panicAct };

        // John's acts
        _gameDevSesh1 = GenerateGameDevSession1();
        _gameDevSesh2 = GenerateGameDevSession2();
        _gameDevAct = GenerateGameDevActivity();
        _gameDevAct.Sessions = new List<Session> { _gameDevSesh1, _gameDevSesh2 };

        _pianoSesh1 = GeneratePianoSession1();
        _pianoSesh2 = GenerateGameDevSession2();
        _pianoAct = GeneratePianoActivity();
        _pianoAct.Sessions = new List<Session> { _pianoSesh1, _pianoSesh2 };

        _mcatAct = GenerateMcatActivity();
        _sleepingAct = GenerateSleepingActivity();
        _johnsActs = new List<Activity> { _gameDevAct, _pianoAct, _mcatAct, _sleepingAct };

        // Judy's acts
        _baseballAct = GenerateBaseballActivity();
        _judysActs = new List<Activity> { _baseballAct };

        _allSessions = new List<Session> { _gameDevSesh1, _gameDevSesh2, _pianoSesh1, _pianoSesh2 };
        _allActs = new List<Activity> { _panicAct, _gameDevAct, _pianoAct, _mcatAct, _sleepingAct, _baseballAct };
    }

    private void _setUpDbMocks()
    {
        _dbContextMock = new Mock<IDataContext>();
        _setUpUsersDbMocks();
        _setUpActivitiesDbMocks();
        _setUpSessionsDbMocks();

        void _setUpUsersDbMocks()
        {
            _usersDbSetMock = _usersData.AsQueryable().BuildMockDbSet();
            _dbContextMock.Setup(x => x.Users)
                    .Returns(_usersDbSetMock.Object);


        }
        void _setUpActivitiesDbMocks()
        {
            _activitiesDbSetMock = _allActs.AsQueryable().BuildMockDbSet();
            _dbContextMock.Setup(x => x.Activities)
                    .Returns(_activitiesDbSetMock.Object);

        }
        void _setUpSessionsDbMocks()
        {
            _sessionsDbSetMock = _allSessions.AsQueryable().BuildMockDbSet();
            _dbContextMock.Setup(x => x.Sessions)
                    .Returns(_sessionsDbSetMock.Object);
        }
    }

    private void _setUpMapperMocks()
    {
        _mapperMock = new Mock<IMapper>();
        _mapperMock.Setup(x => x.Map<ActivityGetDto>(_gameDevAct))
                    .Returns(new ActivityGetDto
                    {
                        Id = _gameDevAct.Id,
                        OwnerId = _gameDevAct.OwnerId,
                        Name = _gameDevAct.Name,
                        Description = _gameDevAct.Description,
                        StartDateUtc = _gameDevAct.StartDateUtc,
                        DueDateUtc = _gameDevAct.DueDateUtc,
                        CompletedDateUtc = _gameDevAct.CompletedDateUtc,
                        IsArchived = _gameDevAct.IsArchived,
                        ColorHex = _gameDevAct.ColorHex,
                        Tags = _gameDevAct.Tags,
                    });
        _mapperMock.Setup(x => x.Map<ActivityGetDto>(_pianoAct))
                    .Returns(new ActivityGetDto
                    {
                        Id = _pianoAct.Id,
                        OwnerId = _pianoAct.OwnerId,
                        Name = _pianoAct.Name,
                        Description = _pianoAct.Description,
                        StartDateUtc = _pianoAct.StartDateUtc,
                        DueDateUtc = _pianoAct.DueDateUtc,
                        CompletedDateUtc = _pianoAct.CompletedDateUtc,
                        IsArchived = _pianoAct.IsArchived,
                        ColorHex = _pianoAct.ColorHex,
                        Tags = _pianoAct.Tags,
                    });
        _mapperMock.Setup(x => x.Map<ActivityGetDto>(_mcatAct))
                    .Returns(new ActivityGetDto
                    {
                        Id = _mcatAct.Id,
                        OwnerId = _mcatAct.OwnerId,
                        Name = _mcatAct.Name,
                        Description = _mcatAct.Description,
                        StartDateUtc = _mcatAct.StartDateUtc,
                        DueDateUtc = _mcatAct.DueDateUtc,
                        CompletedDateUtc = _mcatAct.CompletedDateUtc,
                        IsArchived = _mcatAct.IsArchived,
                        ColorHex = _mcatAct.ColorHex,
                        Tags = _mcatAct.Tags,
                    });
    }

    private void _setUpServiceMocks()
    {
        _userServiceMock = new Mock<IUserService>();
        _sessionServiceMock = new Mock<ISessionService>();
    }

    #region GetAllActivitiesAsync
    [TestMethod]
    public async Task GetAllActivitiesAsync_Admin_AnotherUser_Ok()
    {
        // -- Arrange --
        _userServiceMock.Setup(m => m.IsAdmin(JANE_USER_GUID))
                            .Returns(Task.FromResult(true));

        var activityService = new ActivityService(
            _dbContextMock.Object,
            _userServiceMock.Object,
            _sessionServiceMock.Object,
            _mapperMock.Object);

        // -- Act --
        var activities = await activityService.GetAllActivitiesForUserAsync(JANE_USER_GUID, JOHN_USER_GUID);

        // -- Assert --
        Assert.IsNotNull(activities);
        Assert.AreEqual(activities.Count(), 3);
        var activitiesList = activities.ToList();

        // This list should be ordered by join date, so the activities should be in this order
        _assertActivitiesSame(
            activitiesList[0],
            _gameDevAct.Id,
            _gameDevAct.OwnerId,
            _gameDevAct.Name,
            _gameDevAct.Description,
            _gameDevAct.StartDateUtc,
            _gameDevAct.DueDateUtc,
            _gameDevAct.CompletedDateUtc,
            _gameDevAct.ColorHex,
            _gameDevAct.IsArchived,
            _gameDevAct.Tags);
        _assertActivitiesSame(
            activitiesList[1],
            _pianoAct.Id,
            _pianoAct.OwnerId,
            _pianoAct.Name,
            _pianoAct.Description,
            _pianoAct.StartDateUtc,
            _pianoAct.DueDateUtc,
            _pianoAct.CompletedDateUtc,
            _pianoAct.ColorHex,
            _pianoAct.IsArchived,
            _pianoAct.Tags);
        _assertActivitiesSame(
            activitiesList[2],
            _mcatAct.Id,
            _mcatAct.OwnerId,
            _mcatAct.Name,
            _mcatAct.Description,
            _mcatAct.StartDateUtc,
            _mcatAct.DueDateUtc,
            _mcatAct.CompletedDateUtc,
            _mcatAct.ColorHex,
            _mcatAct.IsArchived,
            _mcatAct.Tags);
    }

    [TestMethod]
    public async Task GetAllActivitiesAsync_NonAdmin_OwnActivities_Ok()
    {
        // -- Arrange --
        _userServiceMock.Setup(m => m.IsAdmin(JOHN_USER_GUID))
                            .Returns(Task.FromResult(false));

        var activityService = new ActivityService(
            _dbContextMock.Object,
            _userServiceMock.Object,
            _sessionServiceMock.Object,
            _mapperMock.Object);

        // -- Act --
        var activities = await activityService.GetAllActivitiesForUserAsync(JOHN_USER_GUID, JOHN_USER_GUID);

        // -- Assert --
        Assert.IsNotNull(activities);
        Assert.AreEqual(activities.Count(), 3);
        var activitiesList = activities.ToList();

        // This list should be ordered by join date, so the activities should be in this order
        _assertActivitiesSame(
            activitiesList[0],
            _gameDevAct.Id,
            _gameDevAct.OwnerId,
            _gameDevAct.Name,
            _gameDevAct.Description,
            _gameDevAct.StartDateUtc,
            _gameDevAct.DueDateUtc,
            _gameDevAct.CompletedDateUtc,
            _gameDevAct.ColorHex,
            _gameDevAct.IsArchived,
            _gameDevAct.Tags);
        _assertActivitiesSame(
            activitiesList[1],
            _pianoAct.Id,
            _pianoAct.OwnerId,
            _pianoAct.Name,
            _pianoAct.Description,
            _pianoAct.StartDateUtc,
            _pianoAct.DueDateUtc,
            _pianoAct.CompletedDateUtc,
            _pianoAct.ColorHex,
            _pianoAct.IsArchived,
            _pianoAct.Tags);
        _assertActivitiesSame(
            activitiesList[2],
            _mcatAct.Id,
            _mcatAct.OwnerId,
            _mcatAct.Name,
            _mcatAct.Description,
            _mcatAct.StartDateUtc,
            _mcatAct.DueDateUtc,
            _mcatAct.CompletedDateUtc,
            _mcatAct.ColorHex,
            _mcatAct.IsArchived,
            _mcatAct.Tags);
    }

    [TestMethod]
    [ExpectedException(typeof(ForbiddenException))]
    public async Task GetAllActivitiesAsync_NonAdmin_AnothersActivities_ThrowForbidden()
    {
        // -- Arrange --
        _userServiceMock.Setup(m => m.IsAdmin(JOHN_USER_GUID))
                            .Returns(Task.FromResult(false));

        var activityService = new ActivityService(
            _dbContextMock.Object,
            _userServiceMock.Object,
            _sessionServiceMock.Object,
            _mapperMock.Object);

        // -- Act --
        var activities = await activityService.GetAllActivitiesForUserAsync(JOHN_USER_GUID, JANE_USER_GUID);
    }

    [TestMethod]
    public async Task GetAllActivitiesAsync_Admin_AnothersActivity_DeletedActivity_ReturnEmpty()
    {
        // -- Arrange --
        _userServiceMock.Setup(m => m.IsAdmin(JANE_USER_GUID))
                            .Returns(Task.FromResult(true));

        var activityService = new ActivityService(
            _dbContextMock.Object,
            _userServiceMock.Object,
            _sessionServiceMock.Object,
            _mapperMock.Object);

        // -- Act --
        var activities = await activityService.GetAllActivitiesForUserAsync(JANE_USER_GUID, JUDY_USER_GUID);

        // -- Assert --
        Assert.AreEqual(activities.Count(), 0);
    }

    #endregion

    #region GetActivityAsync
    [TestMethod]
    public async Task GetActivityAsync_Admin_AnotherUser_Ok()
    {
        // -- Arrange --
        _userServiceMock.Setup(m => m.IsAdmin(JANE_USER_GUID))
                            .Returns(Task.FromResult(true));

        var activityService = new ActivityService(
            _dbContextMock.Object,
            _userServiceMock.Object,
            _sessionServiceMock.Object,
            _mapperMock.Object);

        // -- Act --
        var activity = await activityService.GetActivityAsync(JANE_USER_GUID, GAME_DEV_ACT_GUID);

        // -- Assert --
        Assert.IsNotNull(activity);
        _assertActivitiesSame(
            activity,
            _gameDevAct.Id,
            _gameDevAct.OwnerId,
            _gameDevAct.Name,
            _gameDevAct.Description,
            _gameDevAct.StartDateUtc,
            _gameDevAct.DueDateUtc,
            _gameDevAct.CompletedDateUtc,
            _gameDevAct.ColorHex,
            _gameDevAct.IsArchived,
            _gameDevAct.Tags);
    }

    [TestMethod]
    public async Task GetActivityAsync_NonAdmin_OwnActivity_Ok()
    {
        // -- Arrange --
        _userServiceMock.Setup(m => m.IsAdmin(JOHN_USER_GUID))
                            .Returns(Task.FromResult(false));

        var activityService = new ActivityService(
            _dbContextMock.Object,
            _userServiceMock.Object,
            _sessionServiceMock.Object,
            _mapperMock.Object);

        // -- Act --
        var activity = await activityService.GetActivityAsync(JOHN_USER_GUID, GAME_DEV_ACT_GUID);

        // -- Assert --
        Assert.IsNotNull(activity);
        _assertActivitiesSame(
            activity,
            _gameDevAct.Id,
            _gameDevAct.OwnerId,
            _gameDevAct.Name,
            _gameDevAct.Description,
            _gameDevAct.StartDateUtc,
            _gameDevAct.DueDateUtc,
            _gameDevAct.CompletedDateUtc,
            _gameDevAct.ColorHex,
            _gameDevAct.IsArchived,
            _gameDevAct.Tags);
    }

    [TestMethod]
    [ExpectedException(typeof(ForbiddenException))]
    public async Task GetActivityAsync_NonAdmin_AnothersActivity_ThrowForbidden()
    {
        // -- Arrange --
        _userServiceMock.Setup(m => m.IsAdmin(JOHN_USER_GUID))
                            .Returns(Task.FromResult(false));

        var activityService = new ActivityService(
            _dbContextMock.Object,
            _userServiceMock.Object,
            _sessionServiceMock.Object,
            _mapperMock.Object);

        // -- Act --
        var activity = await activityService.GetActivityAsync(JOHN_USER_GUID, PANIC_ACT_GUID);
    }

    [TestMethod]
    public async Task GetActivityAsync_Admin_AnothersActivity_DeletedActivity_ReturnEmpty()
    {
        // -- Arrange --
        _userServiceMock.Setup(m => m.IsAdmin(JANE_USER_GUID))
                            .Returns(Task.FromResult(false));

        var activityService = new ActivityService(
            _dbContextMock.Object,
            _userServiceMock.Object,
            _sessionServiceMock.Object,
            _mapperMock.Object);

        // -- Act --
        var activity = await activityService.GetActivityAsync(JANE_USER_GUID, SLEEPING_ACT_GUID);

        // -- Assert --
        Assert.IsNull(activity);
    }

    #endregion GetActivityAsync

    #region CreateActivityAsync

    [TestMethod]
    public async Task CreateActivityAsync_Admin_AnothersActivity_Ok()
    {
        // -- Arrange --
        _userServiceMock.Setup(m => m.IsAdmin(JANE_USER_GUID))
                            .Returns(Task.FromResult(true));

        var actCreateDto = new ActivityCreateDto
        {
            Name = "NEW ACT",
            Description = "A new activity",
            StartDateUtc = null,
            DueDateUtc = null,
            CompletedDateUtc = null,
            ColorHex = "#f00000",
            Tags = null
        };

        var actEntity = new Activity
        {
            Id = Guid.NewGuid(),
            OwnerId = JOHN_USER_GUID,
            Name = actCreateDto.Name,
            Description = actCreateDto.Description,
            StartDateUtc = actCreateDto.StartDateUtc,
            DueDateUtc = actCreateDto.DueDateUtc,
            CompletedDateUtc = actCreateDto.CompletedDateUtc,
            IsArchived = false,
            ColorHex = actCreateDto.ColorHex,
            Tags = actCreateDto.Tags,
        };

        _mapperMock.Setup(x => x.Map<Activity>(actCreateDto))
                    .Returns(actEntity);

        _mapperMock.Setup(x => x.Map<ActivityGetDto>(actEntity))
                    .Returns(new ActivityGetDto
                    {
                        Id = actEntity.Id,
                        OwnerId = JOHN_USER_GUID,
                        Name = actEntity.Name,
                        Description = actEntity.Description,
                        StartDateUtc = actEntity.StartDateUtc,
                        DueDateUtc = actEntity.DueDateUtc,
                        CompletedDateUtc = actEntity.CompletedDateUtc,
                        IsArchived = actEntity.IsArchived,
                        ColorHex = actEntity.ColorHex,
                        Tags = actEntity.Tags,
                    });

        var activityService = new ActivityService(
            _dbContextMock.Object,
            _userServiceMock.Object,
            _sessionServiceMock.Object,
            _mapperMock.Object);

        // -- Act --
        var activity = await activityService.CreateActivityAsync(JANE_USER_GUID, JOHN_USER_GUID, actCreateDto);

        // -- Assert --
        _dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Once);
        Assert.IsNotNull(activity);
        Assert.AreEqual(actCreateDto.Name, activity.Name);
        Assert.AreEqual(actCreateDto.Description, activity.Description);
        // TODO: NEED TO WRAP DateTime to be able to mock it
        // Assert.IsTrue(DatesEqualWithinSeconds((DateTime) activity.StartDateUtc, DateTime.UtcNow));
        Assert.IsNull(activity.DueDateUtc);
        Assert.IsNull(activity.CompletedDateUtc);
        Assert.AreEqual(actCreateDto.ColorHex, activity.ColorHex);
        Assert.IsFalse(activity.IsArchived);
        Assert.IsNull(activity.Tags);
    }

    // TODO more tests
    #endregion CreateActivityAsync

    #region UpdateActivityAsync
    // TODO add tests
    #endregion UpdateActivityAsync

    #region DeleteActivityAsync
    [TestMethod]
    public async Task DeleteActivityAsync_Admin_AnothersActivity_Ok()
    {
        // -- Arrange --
        _userServiceMock.Setup(m => m.IsAdmin(JANE_USER_GUID))
                            .Returns(Task.FromResult(true));

        var activityService = new ActivityService(
            _dbContextMock.Object,
            _userServiceMock.Object,
            _sessionServiceMock.Object,
            _mapperMock.Object);

        // -- Act --
        var isSuccessful = await activityService.DeleteActivityAsync(JANE_USER_GUID, GAME_DEV_ACT_GUID);

        // -- Assert --
        Assert.IsTrue(isSuccessful);
        _dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Once);

        // Activity should be soft-deleted
        Assert.IsNotNull(_gameDevAct.DeletedDateUtc);
        Assert.IsTrue(DatesEqualWithinSeconds((DateTime) _gameDevAct.DeletedDateUtc, DateTime.UtcNow));

        // Sessions should be deleted too
        Assert.IsNotNull(_gameDevSesh1.DeletedDateUtc);
        Assert.IsNotNull(_gameDevSesh2.DeletedDateUtc);
        Assert.IsTrue(DatesEqualWithinSeconds((DateTime) _gameDevSesh1.DeletedDateUtc, DateTime.UtcNow));
        Assert.IsTrue(DatesEqualWithinSeconds((DateTime) _gameDevSesh2.DeletedDateUtc, DateTime.UtcNow));

    }

    [TestMethod]
    [ExpectedException(typeof(ForbiddenException))]

    public async Task DeleteActivityAsync_NonAdmin_AnothersActivity_ThrowForbidden()
    {
        // -- Arrange --
        _userServiceMock.Setup(m => m.IsAdmin(JOHN_USER_GUID))
                            .Returns(Task.FromResult(false));

        var activityService = new ActivityService(
            _dbContextMock.Object,
            _userServiceMock.Object,
            _sessionServiceMock.Object,
            _mapperMock.Object);

        // -- Act --
        var isSuccessful = await activityService.DeleteActivityAsync(JOHN_USER_GUID, PANIC_ACT_GUID);
    }

    [TestMethod]

    public async Task DeleteActivityAsync_NonAdmin_OwnActivity_Ok()
    {
        // -- Arrange --
        _userServiceMock.Setup(m => m.IsAdmin(JOHN_USER_GUID))
                            .Returns(Task.FromResult(false));

        var activityService = new ActivityService(
            _dbContextMock.Object,
            _userServiceMock.Object,
            _sessionServiceMock.Object,
            _mapperMock.Object);

        // -- Act --
        var isSuccessful = await activityService.DeleteActivityAsync(JOHN_USER_GUID, GAME_DEV_ACT_GUID);
        
        // -- Assert --
        Assert.IsTrue(isSuccessful);
        _dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Once);

        // Activity should be soft-deleted
        Assert.IsNotNull(_gameDevAct.DeletedDateUtc);
        Assert.IsTrue(DatesEqualWithinSeconds((DateTime) _gameDevAct.DeletedDateUtc, DateTime.UtcNow));

        // Sessions should be deleted too
        Assert.IsNotNull(_gameDevSesh1.DeletedDateUtc);
        Assert.IsNotNull(_gameDevSesh2.DeletedDateUtc);
        Assert.IsTrue(DatesEqualWithinSeconds((DateTime) _gameDevSesh1.DeletedDateUtc, DateTime.UtcNow));
        Assert.IsTrue(DatesEqualWithinSeconds((DateTime) _gameDevSesh2.DeletedDateUtc, DateTime.UtcNow));
    }

    // TODO more tests
    #endregion DeleteActivityAsync

    private void _assertActivitiesSame(
        ActivityGetDto activity,
        Guid id,
        Guid ownerId,
        string name,
        string desc,
        DateTime? startDateUtc,
        DateTime? dueDateUtc,
        DateTime? completedDateUtc,
        string colorHex,
        bool isArchived,
        IList<string>? tags)
    {
        Assert.IsTrue(activity != null);
        Assert.AreEqual(id, activity.Id);
        Assert.AreEqual(ownerId, activity.OwnerId);
        Assert.AreEqual(name, activity.Name);
        Assert.AreEqual(desc, activity.Description);
        Assert.AreEqual(colorHex, activity.ColorHex);
        Assert.AreEqual(isArchived, activity.IsArchived);

        if (tags == null)
        {
            Assert.IsNull(activity.Tags);
        }
        else if (tags != null)
        {
            Assert.IsNotNull(activity.Tags);
            Assert.AreEqual(tags.Count(), activity.Tags.Count());
            foreach (var tag in tags)
            {
                activity.Tags.Contains(tag);
            }
            foreach (var tag in activity.Tags)
            {
                tags.Contains(tag);
            }
        }

        // Check that the dates are equal within a minute
        Assert.IsTrue(DatesEqualWithinSeconds((DateTime)activity.StartDateUtc, DateTime.UtcNow));
        if (activity.DueDateUtc != null)
        {
            Assert.IsTrue(DatesEqualWithinSeconds((DateTime)activity.DueDateUtc, DateTime.UtcNow));
        }
        if (activity.CompletedDateUtc != null)
        {
            Assert.IsTrue(DatesEqualWithinSeconds((DateTime)activity.CompletedDateUtc, DateTime.UtcNow, 60));
        }
    }
}