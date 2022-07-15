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

[TestClass]
public abstract class TestBase
{
    protected static List<User> _usersData;

    protected static Activity _panicAct;
    protected static Session _panicSesh;
    protected static Activity _baseballAct;
    protected static Activity _gameDevAct;
    protected static Session _gameDevSesh1;
    protected static Session _gameDevSesh2;
    protected static Activity _pianoAct;
    protected static Session _pianoSesh1;
    protected static Session _pianoSesh2;
    protected static Activity _mcatAct;
    protected static Activity _sleepingAct;

    protected static List<Activity> _janesActs;
    protected static List<Activity> _johnsActs;
    protected static List<Activity> _judysActs;

    protected static List<Session> _allSessions;
    protected static List<Activity> _allActs;

    protected static Mock<DbSet<User>> _usersDbSetMock;
    protected static Mock<DbSet<Activity>> _activitiesDbSetMock;
    protected static Mock<DbSet<Session>> _sessionsDbSetMock;
    protected static Mock<IDataContext> _dbContextMock;
    protected static Mock<IUserService> _userServiceMock;
    protected static Mock<ISessionService> _sessionServiceMock;
    protected static Mock<IMapper> _mapperMock;

    // TODO figure out why they don't like this
    // // Called before all tests
    // [ClassInitialize()]
    // public static void InitializeClass(TestContext context)
    // {
    //     // Init users here since they won't change through each activity test
    //     _usersData = new List<User> { GenerateJaneUser(), GenerateJohnUser(), GenerateJudyUser() };
    // }

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
        _panicSesh = GeneratePanicSession();
        _panicAct.Sessions = new List<Session>{ _panicSesh };
        _janesActs = new List<Activity> { _panicAct };

        // John's acts
        _gameDevSesh1 = GenerateGameDevSession1();
        _gameDevSesh2 = GenerateGameDevSession2();
        _gameDevAct = GenerateGameDevActivity();
        _gameDevAct.Sessions = new List<Session>{ _gameDevSesh1, _gameDevSesh2 };

        _pianoSesh1 = GeneratePianoSession1();
        _pianoSesh2 = GenerateGameDevSession2();
        _pianoAct = GeneratePianoActivity();
        _pianoAct.Sessions = new List<Session>{ _pianoSesh1, _pianoSesh2 };

        _mcatAct = GenerateMcatActivity();
        _sleepingAct = GenerateSleepingActivity();
        _johnsActs = new List<Activity>{ _gameDevAct, _pianoAct, _mcatAct, _sleepingAct };

        // Judy's acts
        _baseballAct = GenerateBaseballActivity();
        _judysActs = new List<Activity>{ _baseballAct };

        _allSessions = new List<Session>{ _panicSesh, _gameDevSesh1, _gameDevSesh2, _pianoSesh1, _pianoSesh2 };
        _allActs = new List<Activity>{ _panicAct, _gameDevAct, _pianoAct, _mcatAct, _sleepingAct, _baseballAct };
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
}