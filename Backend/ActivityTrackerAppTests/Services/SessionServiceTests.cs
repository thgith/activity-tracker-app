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

// TODO: Need to test diff data edge cases
[TestClass]
public class SessionServiceTests : TestBase
{
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
    [TestCategory("GetAllSessionsByActivityIdAsync")]
    public async Task GetAllSessionsByActivityIdAsync_Admin_AnotherUser_Ok()
    {
        // -- Arrange --
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

        var sessionService = _createSessionService();

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
    [TestCategory("GetAllSessionsByActivityIdAsync")]
    public async Task GetAllSessionsByActivityIdAsync_NonAdmin_OwnSession_Ok()
    {
        // -- Arrange --
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

        var sessionService = _createSessionService();

        // -- Act --
        var sessions = await sessionService.GetAllSessionsByActivityIdAsync(JOHN_USER_GUID, GAME_DEV_ACT_GUID);

        // -- Assert --
        Assert.IsNotNull(sessions);
        Assert.AreEqual(2, sessions.Count());
        var sessionsList = sessions.ToList();
        _assertSessionsEqual(gameDevSesh1, sessionsList[0]);
        _assertSessionsEqual(gameDevSesh2, sessionsList[1]);
    }

    [TestMethod]
    [TestCategory("GetAllSessionsByActivityIdAsync")]
    [TestCategory("Forbidden")]
    [ExpectedException(typeof(ForbiddenException))]
    public async Task GetAllSessionsByActivityIdAsync_NonAdmin_AnothersSession_ThrowForbidden()
    {
        // -- Arrange --
        var sessionService = _createSessionService();

        // -- Act --
        var sessions = await sessionService.GetAllSessionsByActivityIdAsync(JOHN_USER_GUID, PANIC_ACT_GUID);
    }
    #endregion

    #region GetSessionAsync
    [TestMethod]
    [TestCategory("GetSessionAsync")]
    public async Task GetSessionAsync_Admin_AnothersSession_Ok()
    {
        // -- Arrange --
        mapperMock.Setup(x => x.Map<SessionGetDto>(gameDevSesh1))
                    .Returns(new SessionGetDto
                    {
                        Id = gameDevSesh1.Id,
                        ActivityId = gameDevSesh1.ActivityId,
                        StartDateUtc = (DateTime)gameDevSesh1.StartDateUtc,
                        DurationSeconds = gameDevSesh1.DurationSeconds,
                        Notes = gameDevSesh1.Notes
                    });

        var sessionService = _createSessionService();

        // -- Act --
        var session = await sessionService.GetSessionAsync(JANE_USER_GUID, GAME_DEV_SESH1_GUID);

        // -- Assert --
        Assert.IsNotNull(session);
        _assertSessionsEqual(gameDevSesh1, session);
        _assertSessionsEqual(gameDevSesh1, session);
    }

    [TestMethod]
    [TestCategory("GetSessionAsync")]
    public async Task GetSessionAsync_NonAdmin_OwnSession_Ok()
    {
        // -- Arrange --
        mapperMock.Setup(x => x.Map<SessionGetDto>(gameDevSesh1))
                    .Returns(new SessionGetDto
                    {
                        Id = gameDevSesh1.Id,
                        ActivityId = gameDevSesh1.ActivityId,
                        StartDateUtc = (DateTime)gameDevSesh1.StartDateUtc,
                        DurationSeconds = gameDevSesh1.DurationSeconds,
                        Notes = gameDevSesh1.Notes
                    });

        var sessionService = _createSessionService();

        // -- Act --
        var session = await sessionService.GetSessionAsync(JOHN_USER_GUID, GAME_DEV_SESH1_GUID);

        // -- Assert --
        Assert.IsNotNull(session);
        _assertSessionsEqual(gameDevSesh1, session);
        _assertSessionsEqual(gameDevSesh1, session);
    }

    [TestMethod]
    [TestCategory("GetSessionAsync")]
    [TestCategory("Forbidden")]
    [ExpectedException(typeof(ForbiddenException))]
    public async Task GetSessionAsync_NonAdmin_AnothersSession_ThrowForbidden()
    {
        // -- Arrange --
        var sessionService = _createSessionService();

        // -- Act --
        var session = await sessionService.GetSessionAsync(JOHN_USER_GUID, PANIC_SESH_GUID);
    }
    // TODO add more tests
    #endregion GetSessionAsync

    #region CreateSessionAsync
    [TestMethod]
    [TestCategory("CreateSessionAsync")]
    public async Task CreateSessionAsync_Admin_AnothersSession_Ok()
    {
        // -- Arrange --
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

        var sessionService = _createSessionService();

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
    [TestCategory("CreateSessionAsync")]
    [TestCategory("Forbidden")]
    [ExpectedException(typeof(ForbiddenException))]
    public async Task CreateSessionAsync_NonAdmin_AnothersSession_ThrowForbidden()
    {
        // -- Arrange --
        var newSessionDto = new SessionCreateDto
        {
            ActivityId = PANIC_ACT_GUID,
            StartDateUtc = DateTime.UtcNow,
            DurationSeconds = 155,
            Notes = "notes"
        };

        var sessionService = _createSessionService();

        // -- Act --
        var session = await sessionService.CreateSessionAsync(JOHN_USER_GUID, newSessionDto);
    }
    // TODO add more tests
    #endregion CreateSessionAsync

    #region UpdateSessionAsync
    [TestMethod]
    [TestCategory("UpdateSessionAsync")]
    public async Task UpdateSessionAsync_Admin_AnothersSession_Ok()
    {
        // -- Arrange --
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

        var sessionService = _createSessionService();

        // -- Act --
        var session = await sessionService.CreateSessionAsync(JANE_USER_GUID, newSessionDto);

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
    [TestCategory("UpdateSessionAsync")]
    [TestCategory("Forbidden")]
    [ExpectedException(typeof(ForbiddenException))]
    public async Task UpdateSessionAsync_NonAdmin_AnothersSession_ThrowForbidden()
    {
        // -- Arrange --
        var updateSessionDto = new SessionUpdateDto
        {
            StartDateUtc = DateTime.UtcNow,
            DurationSeconds = 200,
            Notes = "notes"
        };

        var sessionService = _createSessionService();

        // -- Act --
        var session = await sessionService.UpdateSessionAsync(JOHN_USER_GUID, PANIC_SESH_GUID, updateSessionDto);
    }

    [TestMethod]
    [TestCategory("UpdateSessionAsync")]
    public async Task UpdateSessionAsync_NonAdmin_OwnSession_Ok()
    {
        // -- Arrange --
        // Replicate the add adding to the DB collection
        sessionsDbSetMock.Setup(m => m.AddAsync(It.IsAny<Session>(), It.IsAny<CancellationToken>()))
                        .Callback((Session session, CancellationToken _) => { allSessions.Add(session); });

        // Replicate the save setting a new GUID for the new session
        dbContextMock.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                        .Callback((CancellationToken _) => { allSessions.Last().Id = Guid.NewGuid(); });

        var updateSessionDto = new SessionUpdateDto
        {
            StartDateUtc = DateTime.UtcNow,
            DurationSeconds = 200,
            Notes = "notes"
        };

        mapperMock.Setup(x => x.Map<Session>(updateSessionDto))
                    .Returns(new Session
                    {
                        StartDateUtc = (DateTime)updateSessionDto.StartDateUtc,
                        DurationSeconds = (uint)updateSessionDto.DurationSeconds,
                        Notes = updateSessionDto.Notes
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

        var sessionService = _createSessionService();

        // -- Act --
        var session = await sessionService.UpdateSessionAsync(JOHN_USER_GUID, GAME_DEV_SESH1_GUID, updateSessionDto);

        // -- Assert --
        // Check return object
        Assert.IsNotNull(session);
        Assert.AreEqual(GAME_DEV_ACT_GUID, session.ActivityId);
        DatesEqualWithinSeconds((DateTime)updateSessionDto.StartDateUtc, session.StartDateUtc);
        Assert.AreEqual(updateSessionDto.DurationSeconds, session.DurationSeconds);
        Assert.AreEqual(updateSessionDto.Notes, session.Notes);

        // Check the fake DB
        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Once);
        Assert.AreEqual(5, allSessions.Count());
        var savedGameDevSesh = allSessions[1];
        Assert.AreEqual(GAME_DEV_ACT_GUID, savedGameDevSesh.ActivityId);
        DatesEqualWithinSeconds((DateTime)updateSessionDto.StartDateUtc, (DateTime)savedGameDevSesh.StartDateUtc);
        Assert.AreEqual(updateSessionDto.DurationSeconds, savedGameDevSesh.DurationSeconds);
        Assert.AreEqual(updateSessionDto.Notes, savedGameDevSesh.Notes);
    }

    // TODO add more tests
    #endregion UpdateSessionAsync

    #region DeleteSessionAsync
    [TestMethod]
    [TestCategory("DeleteSessionAsync")]

    public async Task DeleteActivityAsync_Admin_AnothersSession_Ok()
    {
        // -- Arrange --
        var sessionService = _createSessionService();

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
    [TestCategory("DeleteSessionAsync")]
    [TestCategory("Forbidden")]
    [ExpectedException(typeof(ForbiddenException))]
    public async Task DeleteActivityAsync_NonAdmin_AnothersSession_ThrowForbidden()
    {
        // -- Arrange --
        var sessionService = _createSessionService();

        // -- Act --
        var isSuccessful = await sessionService.DeleteSessionAsync(JOHN_USER_GUID, PANIC_SESH_GUID);
    }

    [TestMethod]
    [TestCategory("DeleteActivityAsync")]

    public async Task DeleteActivityAsync_NonAdmin_OwnActivity_Ok()
    {
        // -- Arrange --
        var sessionService = _createSessionService();

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

    private SessionService _createSessionService()
    {
        return new SessionService(
            dbContextMock.Object,
            userServiceMock.Object,
            mapperMock.Object);
    }

    private void _assertSessionsEqual(Session expectedSession, SessionGetDto actualSession)
    {
        Assert.IsNotNull(actualSession);
        Assert.AreEqual(expectedSession.Id, actualSession.Id);
        Assert.AreEqual(expectedSession.ActivityId, actualSession.ActivityId);
        DatesEqualWithinSeconds((DateTime)expectedSession.StartDateUtc, actualSession.StartDateUtc);
        Assert.AreEqual(expectedSession.DurationSeconds, actualSession.DurationSeconds);
        Assert.AreEqual(expectedSession.Notes, actualSession.Notes);
    }
}
