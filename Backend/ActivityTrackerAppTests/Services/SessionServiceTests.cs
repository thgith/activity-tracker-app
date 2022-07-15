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

// TODO: Need to finish these tests
[TestClass]
public class SessionServiceTests : TestBase
{
    private void _assertSessionsEqual(Session expectedSession, SessionGetDto actualSession)
    {
        Assert.IsNotNull(actualSession);
        Assert.AreEqual(expectedSession.Id, actualSession.Id);
        Assert.AreEqual(expectedSession.ActivityId, actualSession.ActivityId);
        DatesEqualWithinSeconds((DateTime)expectedSession.StartDateUtc, actualSession.StartDateUtc);
        Assert.AreEqual(expectedSession.DurationSeconds, actualSession.DurationSeconds);
        Assert.AreEqual(expectedSession.Notes, actualSession.Notes);
    }

    // Called before all tests
    // TODO figure out why it didn't like being in the base class
    [ClassInitialize()]
    public static void InitializeClass(TestContext context)
    {
        // Init users here since they won't change through each activity test
        usersData = new List<User> { GenerateJaneUser(), GenerateJohnUser(), GenerateJudyUser() };
    }

    #region GetAllSessionsAsync
    [TestMethod]
    public async Task GetAllSessionsByActivityIdAsync_Admin_AnotherUser_Ok()
    {
        // -- Arrange --
        userServiceMock.Setup(m => m.IsAdmin(JANE_USER_GUID))
                            .Returns(Task.FromResult(true));

        mapperMock.Setup(x => x.Map<SessionGetDto>(gameDevSesh1))
                    .Returns(new SessionGetDto
                    {
                        Id = gameDevSesh1.Id,
                        ActivityId = gameDevSesh1.ActivityId,
                        StartDateUtc = (DateTime)gameDevSesh1.StartDateUtc,
                        DurationSeconds = gameDevSesh1.DurationSeconds,
                        Notes = gameDevSesh1.Notes
                    });

        mapperMock.Setup(x => x.Map<SessionGetDto>(gameDevSesh2))
                    .Returns(new SessionGetDto
                    {
                        Id = gameDevSesh2.Id,
                        ActivityId = gameDevSesh2.ActivityId,
                        StartDateUtc = (DateTime)gameDevSesh2.StartDateUtc,
                        DurationSeconds = gameDevSesh2.DurationSeconds,
                        Notes = gameDevSesh2.Notes
                    });

        var sessionService = new SessionService(
            dbContextMock.Object,
            userServiceMock.Object,
            mapperMock.Object);

        // -- Act --
        var sessions = await sessionService.GetAllSessionsByActivityIdAsync(JANE_USER_GUID, GAME_DEV_ACT_GUID);

        // -- Assert --
        Assert.IsNotNull(sessions);
        Assert.AreEqual(2, sessions.Count());
        var sessionsList = sessions.ToList();
        _assertSessionsEqual(gameDevSesh1, sessionsList[0]);
        _assertSessionsEqual(gameDevSesh2, sessionsList[1]);
    }

    [TestMethod]
    public async Task GetAllSessionsByActivityIdAsync_NonAdmin_OwnSession_Ok()
    {
        // -- Arrange --
        userServiceMock.Setup(m => m.IsAdmin(JOHN_USER_GUID))
                            .Returns(Task.FromResult(false));

        mapperMock.Setup(x => x.Map<SessionGetDto>(gameDevSesh1))
                    .Returns(new SessionGetDto
                    {
                        Id = gameDevSesh1.Id,
                        ActivityId = gameDevSesh1.ActivityId,
                        StartDateUtc = (DateTime)gameDevSesh1.StartDateUtc,
                        DurationSeconds = gameDevSesh1.DurationSeconds,
                        Notes = gameDevSesh1.Notes
                    });

        mapperMock.Setup(x => x.Map<SessionGetDto>(gameDevSesh2))
                    .Returns(new SessionGetDto
                    {
                        Id = gameDevSesh2.Id,
                        ActivityId = gameDevSesh2.ActivityId,
                        StartDateUtc = (DateTime)gameDevSesh2.StartDateUtc,
                        DurationSeconds = gameDevSesh2.DurationSeconds,
                        Notes = gameDevSesh2.Notes
                    });

        var sessionService = new SessionService(
            dbContextMock.Object,
            userServiceMock.Object,
            mapperMock.Object);

        // -- Act --
        var sessions = await sessionService.GetAllSessionsByActivityIdAsync(JOHN_USER_GUID, GAME_DEV_ACT_GUID);

        // -- Assert --
        Assert.IsNotNull(sessions);
        Assert.AreEqual(2, sessions.Count());
        var sessionsList = sessions.ToList();
        _assertSessionsEqual(gameDevSesh1, sessionsList[0]);
        _assertSessionsEqual(gameDevSesh2, sessionsList[1]);
    }
    #endregion

    #region GetSessionAsync
    [TestMethod]
    public async Task GetSessionAsync_Admin_AnothersSession_Ok()
    {
        // -- Arrange --
        userServiceMock.Setup(m => m.IsAdmin(JANE_USER_GUID))
                            .Returns(Task.FromResult(true));

        mapperMock.Setup(x => x.Map<SessionGetDto>(gameDevSesh1))
                    .Returns(new SessionGetDto
                    {
                        Id = gameDevSesh1.Id,
                        ActivityId = gameDevSesh1.ActivityId,
                        StartDateUtc = (DateTime)gameDevSesh1.StartDateUtc,
                        DurationSeconds = gameDevSesh1.DurationSeconds,
                        Notes = gameDevSesh1.Notes
                    });

        var sessionService = new SessionService(
            dbContextMock.Object,
            userServiceMock.Object,
            mapperMock.Object);

        // -- Act --
        var session = await sessionService.GetSessionAsync(JANE_USER_GUID, GAME_DEV_SESH1_GUID);

        // -- Assert --
        Assert.IsNotNull(session);
        _assertSessionsEqual(gameDevSesh1, session);
        _assertSessionsEqual(gameDevSesh1, session);
    }

    [TestMethod]
    public async Task GetSessionAsync_NonAdmin_OwnSession_Ok()
    {
        // -- Arrange --
        userServiceMock.Setup(m => m.IsAdmin(JOHN_USER_GUID))
                            .Returns(Task.FromResult(false));

        mapperMock.Setup(x => x.Map<SessionGetDto>(gameDevSesh1))
                    .Returns(new SessionGetDto
                    {
                        Id = gameDevSesh1.Id,
                        ActivityId = gameDevSesh1.ActivityId,
                        StartDateUtc = (DateTime)gameDevSesh1.StartDateUtc,
                        DurationSeconds = gameDevSesh1.DurationSeconds,
                        Notes = gameDevSesh1.Notes
                    });

        var sessionService = new SessionService(
            dbContextMock.Object,
            userServiceMock.Object,
            mapperMock.Object);

        // -- Act --
        var session = await sessionService.GetSessionAsync(JOHN_USER_GUID, GAME_DEV_SESH1_GUID);

        // -- Assert --
        Assert.IsNotNull(session);
        _assertSessionsEqual(gameDevSesh1, session);
        _assertSessionsEqual(gameDevSesh1, session);
    }

    [TestMethod]
    [ExpectedException(typeof(ForbiddenException))]
    public async Task GetSessionAsync_NonAdmin_AnothersSession_ThrowForbidden()
    {
        // -- Arrange --
        userServiceMock.Setup(m => m.IsAdmin(JOHN_USER_GUID))
                            .Returns(Task.FromResult(false));

        var sessionService = new SessionService(
            dbContextMock.Object,
            userServiceMock.Object,
            mapperMock.Object);

        // -- Act --
        var session = await sessionService.GetSessionAsync(JOHN_USER_GUID, PANIC_SESH_GUID);
    }
    // TODO add more tests
    #endregion GetSessionAsync

    #region CreateSessionAsync
    [TestMethod]
    public async Task CreateSessionAsync_Admin_AnothersSession_Ok()
    {
        // -- Arrange --
        userServiceMock.Setup(m => m.IsAdmin(JOHN_USER_GUID))
                            .Returns(Task.FromResult(false));

        var newSessionDto = new SessionCreateDto
        {
            ActivityId = GAME_DEV_ACT_GUID,
            StartDateUtc = DateTime.UtcNow,
            DurationSeconds = 155,
            Notes = "notes"
        };

        // Replicate the add adding to the DB collection
        sessionsDbSetMock.Setup(m => m.AddAsync(It.IsAny<Session>(), It.IsAny<CancellationToken>()))
                        .Callback((Session session, CancellationToken _) => { allSessions.Add(session); });

        // Replicate the save setting a new GUID for the new session
        dbContextMock.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                        .Callback((CancellationToken _) => { allSessions.Last().Id = Guid.NewGuid(); });

        mapperMock.Setup(x => x.Map<Session>(newSessionDto))
                    .Returns(new Session
                    {
                        ActivityId = newSessionDto.ActivityId,
                        StartDateUtc = (DateTime)newSessionDto.StartDateUtc,
                        DurationSeconds = newSessionDto.DurationSeconds,
                        Notes = newSessionDto.Notes
                    });

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

        var sessionService = new SessionService(
            dbContextMock.Object,
            userServiceMock.Object,
            mapperMock.Object);

        // -- Act --
        var session = await sessionService.CreateSessionAsync(JOHN_USER_GUID, newSessionDto);

        // -- Assert --
        // Check return object
        Assert.IsNotNull(session);
        Assert.IsNotNull(session.Id);
        Assert.AreNotEqual(Guid.Empty, session.Id);
        Assert.AreEqual(newSessionDto.ActivityId, session.ActivityId);
        DatesEqualWithinSeconds((DateTime)newSessionDto.StartDateUtc, session.StartDateUtc);
        Assert.AreEqual(newSessionDto.DurationSeconds, session.DurationSeconds);
        Assert.AreEqual(newSessionDto.Notes, session.Notes);

        // Check the fake DB
        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Once);
        Assert.AreEqual(6, allSessions.Count());
        var savedSesh = allSessions.Last();
        Assert.IsNotNull(savedSesh.Id);
        Assert.AreNotEqual(Guid.Empty, savedSesh.Id);
        Assert.AreEqual(newSessionDto.ActivityId, savedSesh.ActivityId);
        DatesEqualWithinSeconds((DateTime)newSessionDto.StartDateUtc, (DateTime)savedSesh.StartDateUtc);
        Assert.AreEqual(newSessionDto.DurationSeconds, savedSesh.DurationSeconds);
        Assert.AreEqual(newSessionDto.Notes, savedSesh.Notes);
    }

    [TestMethod]
    [ExpectedException(typeof(ForbiddenException))]
    public async Task CreateSessionAsync_NonAdmin_AnothersSession_ThrowForbidden()
    {
        // -- Arrange --
        userServiceMock.Setup(m => m.IsAdmin(JOHN_USER_GUID))
                            .Returns(Task.FromResult(false));

        var newSessionDto = new SessionCreateDto
        {
            ActivityId = PANIC_ACT_GUID,
            StartDateUtc = DateTime.UtcNow,
            DurationSeconds = 155,
            Notes = "notes"
        };

        var sessionService = new SessionService(
            dbContextMock.Object,
            userServiceMock.Object,
            mapperMock.Object);

        // -- Act --
        var session = await sessionService.CreateSessionAsync(JOHN_USER_GUID, newSessionDto);
    }

    [TestMethod]
    public async Task CreateSessionAsync_NonAdmin_OwnSession()
    {
        // -- Arrange --
        userServiceMock.Setup(m => m.IsAdmin(JOHN_USER_GUID))
                            .Returns(Task.FromResult(false));

        // Replicate the add adding to the DB collection
        sessionsDbSetMock.Setup(m => m.AddAsync(It.IsAny<Session>(), It.IsAny<CancellationToken>()))
                        .Callback((Session session, CancellationToken _) => { allSessions.Add(session); });

        // Replicate the save setting a new GUID for the new session
        dbContextMock.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                        .Callback((CancellationToken _) => { allSessions.Last().Id = Guid.NewGuid(); });

        var newSessionDto = new SessionCreateDto
        {
            ActivityId = GAME_DEV_ACT_GUID,
            StartDateUtc = DateTime.UtcNow,
            DurationSeconds = 155,
            Notes = "notes"
        };

        mapperMock.Setup(x => x.Map<Session>(newSessionDto))
                    .Returns(new Session
                    {
                        ActivityId = newSessionDto.ActivityId,
                        StartDateUtc = (DateTime)newSessionDto.StartDateUtc,
                        DurationSeconds = newSessionDto.DurationSeconds,
                        Notes = newSessionDto.Notes
                    });

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

        var sessionService = new SessionService(
            dbContextMock.Object,
            userServiceMock.Object,
            mapperMock.Object);

        // -- Act --
        var session = await sessionService.CreateSessionAsync(JOHN_USER_GUID, newSessionDto);

        // -- Assert --
        // Check return object
        Assert.IsNotNull(session);
        Assert.IsNotNull(session.Id);
        Assert.AreNotEqual(Guid.Empty, session.Id);
        Assert.AreEqual(newSessionDto.ActivityId, session.ActivityId);
        DatesEqualWithinSeconds((DateTime)newSessionDto.StartDateUtc, session.StartDateUtc);
        Assert.AreEqual(newSessionDto.DurationSeconds, session.DurationSeconds);
        Assert.AreEqual(newSessionDto.Notes, session.Notes);

        // Check the fake DB
        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Once);
        Assert.AreEqual(6, allSessions.Count());
        var savedSesh = allSessions.Last();
        Assert.IsNotNull(savedSesh.Id);
        Assert.AreNotEqual(Guid.Empty, savedSesh.Id);
        Assert.AreEqual(newSessionDto.ActivityId, savedSesh.ActivityId);
        DatesEqualWithinSeconds((DateTime)newSessionDto.StartDateUtc, (DateTime)savedSesh.StartDateUtc);
        Assert.AreEqual(newSessionDto.DurationSeconds, savedSesh.DurationSeconds);
        Assert.AreEqual(newSessionDto.Notes, savedSesh.Notes);
    }

    // TODO add more tests
    #endregion CreateSessionAsync

    #region UpdateSessionAsync
    #endregion UpdateSessionAsync

    #region DeleteSessionAsync
    [TestMethod]

    public async Task DeleteActivityAsync_Admin_AnothersSession_Ok()
    {
        // -- Arrange --
        userServiceMock.Setup(m => m.IsAdmin(JANE_USER_GUID))
                            .Returns(Task.FromResult(true));

        var sessionService = new SessionService(
            dbContextMock.Object,
            userServiceMock.Object,
            mapperMock.Object);

        // -- Act --
        var isSuccessful = await sessionService.DeleteSessionAsync(JANE_USER_GUID, GAME_DEV_SESH1_GUID);

        // -- Assert --
        Assert.IsTrue(isSuccessful);
        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Once);

        // Sessions should be soft-deleted
        Assert.IsNotNull(gameDevSesh1.DeletedDateUtc);
        Assert.IsTrue(DatesEqualWithinSeconds((DateTime)gameDevSesh1.DeletedDateUtc, DateTime.UtcNow));

        // The session's activity should not be soft-deleted
        Assert.IsNull(gameDevAct.DeletedDateUtc);
    }

    [TestMethod]
    [ExpectedException(typeof(ForbiddenException))]

    public async Task DeleteActivityAsync_NonAdmin_AnothersSession_ThrowForbidden()
    {
        // -- Arrange --
        userServiceMock.Setup(m => m.IsAdmin(JOHN_USER_GUID))
                            .Returns(Task.FromResult(false));

        var sessionService = new SessionService(
            dbContextMock.Object,
            userServiceMock.Object,
            mapperMock.Object);

        // -- Act --
        var isSuccessful = await sessionService.DeleteSessionAsync(JOHN_USER_GUID, PANIC_SESH_GUID);
    }

    [TestMethod]

    public async Task DeleteActivityAsync_NonAdmin_OwnActivity_Ok()
    {
        // -- Arrange --
        userServiceMock.Setup(m => m.IsAdmin(JOHN_USER_GUID))
                            .Returns(Task.FromResult(false));

        var sessionService = new SessionService(
            dbContextMock.Object,
            userServiceMock.Object,
            mapperMock.Object);

        // -- Act --
        var isSuccessful = await sessionService.DeleteSessionAsync(JOHN_USER_GUID, GAME_DEV_SESH1_GUID);

        // -- Assert --
        Assert.IsTrue(isSuccessful);
        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Once);

        // Session should be soft-deleted
        Assert.IsNotNull(gameDevSesh1.DeletedDateUtc);
        Assert.IsTrue(DatesEqualWithinSeconds((DateTime)gameDevSesh1.DeletedDateUtc, DateTime.UtcNow));

        // The session's activity should not be soft-deleted
        Assert.IsNull(gameDevAct.DeletedDateUtc);
    }
    #endregion DeleteSessionAsync
}
