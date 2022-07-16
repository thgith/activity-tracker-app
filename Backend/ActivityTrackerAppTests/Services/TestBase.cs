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
    protected static List<User> usersData;

    protected static Activity panicAct;
    protected static Session panicSesh;
    protected static Activity baseballAct;
    protected static Activity gameDevAct;
    protected static Session gameDevSesh1;
    protected static Session gameDevSesh2;
    protected static Activity pianoAct;
    protected static Session pianoSesh1;
    protected static Session pianoSesh2;
    protected static Activity mcatAct;
    protected static Activity sleepingAct;

    protected static List<Activity> janesActs;
    protected static List<Activity> johnsActs;
    protected static List<Activity> judysActs;

    protected static List<Session> allSessions;
    protected static List<Activity> allActs;

    protected static Mock<DbSet<User>> usersDbSetMock;
    protected static Mock<DbSet<Activity>> activitiesDbSetMock;
    protected static Mock<DbSet<Session>> sessionsDbSetMock;
    protected static Mock<IDataContext> dbContextMock;
    protected static Mock<IUserService> userServiceMock;
    protected static Mock<ISessionService> sessionServiceMock;
    protected static Mock<IMapper> mapperMock;

    // TODO figure out why they don't like this
    // Called before all tests
    // [ClassInitialize()]
    // public static void InitializeClass(TestContext context)
    // {
    //     // Init users here since they won't change through each activity test
    //     usersData = new List<User> { GenerateJaneUser(), GenerateJohnUser(), GenerateJudyUser() };
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
        panicAct = GeneratePanicActivity();
        panicSesh = GeneratePanicSession();
        panicAct.Sessions = new List<Session> { panicSesh };
        janesActs = new List<Activity> { panicAct };

        // John's acts
        gameDevSesh1 = GenerateGameDevSession1();
        gameDevSesh2 = GenerateGameDevSession2();
        gameDevAct = GenerateGameDevActivity();
        gameDevAct.Sessions = new List<Session> { gameDevSesh1, gameDevSesh2 };

        pianoSesh1 = GeneratePianoSession1();
        pianoSesh2 = GeneratePianoSession2();
        pianoAct = GeneratePianoActivity();
        pianoAct.Sessions = new List<Session> { pianoSesh1, pianoSesh2 };

        mcatAct = GenerateMcatActivity();
        sleepingAct = GenerateSleepingActivity();
        johnsActs = new List<Activity> { gameDevAct, pianoAct, mcatAct, sleepingAct };

        // Judy's acts
        baseballAct = GenerateBaseballActivity();
        judysActs = new List<Activity> { baseballAct };

        allSessions = new List<Session> { panicSesh, gameDevSesh1, gameDevSesh2, pianoSesh1, pianoSesh2 };
        allActs = new List<Activity> { panicAct, gameDevAct, pianoAct, mcatAct, sleepingAct, baseballAct };
    }

    private void _setUpDbMocks()
    {
        dbContextMock = new Mock<IDataContext>();
        _setUpUsersDbMocks();
        _setUpActivitiesDbMocks();
        _setUpSessionsDbMocks();

        void _setUpUsersDbMocks()
        {
            usersDbSetMock = usersData.AsQueryable().BuildMockDbSet();
            dbContextMock.Setup(x => x.Users)
                    .Returns(usersDbSetMock.Object);
        }

        void _setUpActivitiesDbMocks()
        {
            activitiesDbSetMock = allActs.AsQueryable().BuildMockDbSet();
            dbContextMock.Setup(x => x.Activities)
                    .Returns(activitiesDbSetMock.Object);

        }

        void _setUpSessionsDbMocks()
        {
            sessionsDbSetMock = allSessions.AsQueryable().BuildMockDbSet();
            dbContextMock.Setup(x => x.Sessions)
                    .Returns(sessionsDbSetMock.Object);
        }
    }

    private void _setUpMapperMocks()
    {
        mapperMock = new Mock<IMapper>();

        // Activity Get mapping
        mapperMock.Setup(x => x.Map<ActivityGetDto>(It.Is<Activity>(x => x == null)))
            .Returns<ActivityGetDto>(null);
        mapperMock.Setup(x => x.Map<ActivityGetDto>(It.Is<Activity>(x => x != null)))
            .Returns((Activity activity) =>
                new ActivityGetDto
                {
                    Id = activity.Id,
                    OwnerId = activity.OwnerId,
                    Name = activity.Name,
                    Description = activity.Description,
                    StartDateUtc = activity.StartDateUtc,
                    DueDateUtc = activity.DueDateUtc,
                    CompletedDateUtc = activity.CompletedDateUtc,
                    IsArchived = activity.IsArchived,
                    ColorHex = activity.ColorHex,
                    Tags = activity.Tags
                });

        // Activity Create mapping
        mapperMock.Setup(x => x.Map<Activity>(It.Is<ActivityCreateDto>(x => x == null)))
                    .Returns<Activity>(null);
        mapperMock.Setup(x => x.Map<Activity>(It.Is<ActivityCreateDto>(x => x != null)))
                    .Returns((ActivityCreateDto actCreateDto) =>
                    new Activity
                    {
                        Name = actCreateDto.Name,
                        Description = actCreateDto.Description,
                        StartDateUtc = actCreateDto.StartDateUtc,
                        DueDateUtc = actCreateDto.DueDateUtc,
                        CompletedDateUtc = actCreateDto.CompletedDateUtc,
                        IsArchived = false,
                        ColorHex = actCreateDto.ColorHex,
                        Tags = actCreateDto.Tags,
                    });

        // Session Get mapping
        mapperMock.Setup(x => x.Map<Session>(It.Is<Session>(x => x == null)))
            .Returns<SessionGetDto>(null);
        mapperMock.Setup(x => x.Map<SessionGetDto>(It.IsAny<Session>()))
                    .Returns((Session session) =>
                        new SessionGetDto
                        {
                            Id = session.Id,
                            ActivityId = session.ActivityId,
                            StartDateUtc = (DateTime)session.StartDateUtc,
                            DurationSeconds = session.DurationSeconds,
                            Notes = session.Notes
                        });
                        
        // Session Create mapping
        mapperMock.Setup(x => x.Map<Session>(It.Is<SessionCreateDto>(x => x == null)))
            .Returns<Session>(null);
        mapperMock.Setup(x => x.Map<Session>(It.Is<SessionCreateDto>(x => x != null)))
            .Returns((SessionCreateDto sessionCreateDto) =>
                new Session
                {
                    ActivityId = sessionCreateDto.ActivityId,
                    StartDateUtc = (DateTime)sessionCreateDto.StartDateUtc,
                    DurationSeconds = sessionCreateDto.DurationSeconds,
                    Notes = sessionCreateDto.Notes
                });
    }

    private void _setUpServiceMocks()
    {
        userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(m => m.IsAdmin(JANE_USER_GUID))
                    .Returns(Task.FromResult(true));
        userServiceMock.Setup(m => m.IsAdmin(JOHN_USER_GUID))
                            .Returns(Task.FromResult(false));
        userServiceMock.Setup(m => m.IsAdmin(JUDY_USER_GUID))
                            .Returns(Task.FromResult(false));

        sessionServiceMock = new Mock<ISessionService>();
    }
}