using ActivityTrackerApp.Entities;
using ActivityTrackerApp.Services;
using static ActivityTrackerAppTests.Fixtures.TestFixtures;
using static ActivityTrackerAppTests.Helpers.TestHelpers;
using Moq;
using ActivityTrackerApp.Dtos;
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
        var sessionService = _createSessionService();

        // -- Act --
        var sessions = await sessionService.GetAllSessionsByActivityIdAsync(JANE_USER_GUID, GAME_DEV_ACT_GUID);

        // -- Assert --
        Assert.IsNotNull(sessions);
        Assert.AreEqual(2, sessions.Count());
        var sessionsList = sessions.ToList();
        _assertSessionsEqual(gameDevSesh1, sessionsList[0]);
        _assertSessionsEqual(gameDevSesh2, sessionsList[1]);

        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Never);
    }

    [TestMethod]
    [TestCategory("GetAllSessionsByActivityIdAsync")]
    public async Task GetAllSessionsByActivityIdAsync_NonAdmin_OwnSession_Ok()
    {
        // -- Arrange --
        var sessionService = _createSessionService();

        // -- Act --
        var sessions = await sessionService.GetAllSessionsByActivityIdAsync(JOHN_USER_GUID, GAME_DEV_ACT_GUID);

        // -- Assert --
        Assert.IsNotNull(sessions);
        Assert.AreEqual(2, sessions.Count());
        var sessionsList = sessions.ToList();
        _assertSessionsEqual(gameDevSesh1, sessionsList[0]);
        _assertSessionsEqual(gameDevSesh2, sessionsList[1]);

        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Never);
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
        var sessionService = _createSessionService();

        // -- Act --
        var session = await sessionService.GetSessionAsync(JANE_USER_GUID, GAME_DEV_SESH1_GUID);

        // -- Assert --
        Assert.IsNotNull(session);
        _assertSessionsEqual(gameDevSesh1, session);
        _assertSessionsEqual(gameDevSesh1, session);

        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Never);
    }

    [TestMethod]
    [TestCategory("GetSessionAsync")]
    public async Task GetSessionAsync_NonAdmin_OwnSession_Ok()
    {
        // -- Arrange --
        var sessionService = _createSessionService();

        // -- Act --
        var session = await sessionService.GetSessionAsync(JOHN_USER_GUID, GAME_DEV_SESH1_GUID);

        // -- Assert --
        Assert.IsNotNull(session);
        _assertSessionsEqual(gameDevSesh1, session);
        _assertSessionsEqual(gameDevSesh1, session);

        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Never);
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
    [DataRow(GAME_DEV_ACT_GUID_STR, "", (uint)0, "")]
    [DataRow(GAME_DEV_ACT_GUID_STR, "", (uint)60, SHORT_GENERIC_NOTES)]
    [DataRow(GAME_DEV_ACT_GUID_STR, "", (uint)200, SHORT_GENERIC_NOTES)]
    [DataRow(GAME_DEV_ACT_GUID_STR, "", (uint)999999999, SHORT_GENERIC_NOTES)]
    [DataRow(GAME_DEV_ACT_GUID_STR, "2022-07-15T01:27:26Z", (uint)200, SHORT_GENERIC_NOTES)]
    [DataRow(GAME_DEV_ACT_GUID_STR, MIN_DATE_STR, (uint)200, SHORT_GENERIC_NOTES)]
    [DataRow(GAME_DEV_ACT_GUID_STR, "2025-07-15T01:27:26Z", (uint)200, SHORT_GENERIC_NOTES)]
    [TestCategory("CreateSessionAsync")]
    public async Task CreateSessionAsync_Admin_AnothersSession_Ok(
        string activityIdStr,
        string startDateUtcStr,
        uint durationSeconds,
        string notes)
    {
        // -- Arrange --
        Guid.TryParse(activityIdStr, out var activityGuid);
        DateTime.TryParse(startDateUtcStr, out var startDateUtc);
        var newSessionDto = new SessionCreateDto
        {
            ActivityId = activityGuid,
            StartDateUtc = startDateUtc,
            DurationSeconds = durationSeconds,
            Notes = notes
        };

        // Replicate the add adding to the DB collection
        sessionsDbSetMock.Setup(m => m.AddAsync(It.IsAny<Session>(), It.IsAny<CancellationToken>()))
                        .Callback((Session session, CancellationToken _) => { allSessions.Add(session); });

        // Replicate the save setting a new GUID for the new session
        dbContextMock.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                        .Callback((CancellationToken _) => { allSessions.Last().Id = Guid.NewGuid(); });

        var sessionService = _createSessionService();

        // -- Act --
        var session = await sessionService.CreateSessionAsync(JANE_USER_GUID, newSessionDto);

        // -- Assert --
        // Check return object
        _assertReturnedFromCreate(newSessionDto, session);

        // Check the fake DB
        _assertDbFromCreate(newSessionDto);
    }
    [TestMethod]
    [DataRow(GAME_DEV_ACT_GUID_STR, "", (uint)0, "")]
    [DataRow(GAME_DEV_ACT_GUID_STR, "", (uint)60, SHORT_GENERIC_NOTES)]
    [DataRow(GAME_DEV_ACT_GUID_STR, "", (uint)200, SHORT_GENERIC_NOTES)]
    [DataRow(GAME_DEV_ACT_GUID_STR, "", (uint)999999999, SHORT_GENERIC_NOTES)]
    [DataRow(GAME_DEV_ACT_GUID_STR, "2022-07-15T01:27:26Z", (uint)200, SHORT_GENERIC_NOTES)]
    [DataRow(GAME_DEV_ACT_GUID_STR, MIN_DATE_STR, (uint)200, SHORT_GENERIC_NOTES)]
    [DataRow(GAME_DEV_ACT_GUID_STR, "2025-07-15T01:27:26Z", (uint)200, SHORT_GENERIC_NOTES)]
    [TestCategory("CreateSessionAsync")]
    public async Task CreateSessionAsync_NonAdmin_OwnSession_Ok(
        string activityIdStr,
        string startDateUtcStr,
        uint durationSeconds,
        string notes)
    {
        // -- Arrange --
        Guid.TryParse(activityIdStr, out var activityGuid);
        DateTime.TryParse(startDateUtcStr, out var startDateUtc);
        var newSessionDto = new SessionCreateDto
        {
            ActivityId = activityGuid,
            StartDateUtc = startDateUtc,
            DurationSeconds = durationSeconds,
            Notes = notes
        };

        // Replicate the add adding to the DB collection
        sessionsDbSetMock.Setup(m => m.AddAsync(It.IsAny<Session>(), It.IsAny<CancellationToken>()))
                        .Callback((Session session, CancellationToken _) => { allSessions.Add(session); });

        // Replicate the save setting a new GUID for the new session
        dbContextMock.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                        .Callback((CancellationToken _) => { allSessions.Last().Id = Guid.NewGuid(); });

        var sessionService = _createSessionService();

        // -- Act --
        var session = await sessionService.CreateSessionAsync(JOHN_USER_GUID, newSessionDto);

        // -- Assert --
        // Check return object
        _assertReturnedFromCreate(newSessionDto, session);

        // Check the fake DB
        _assertDbFromCreate(newSessionDto);
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
    [DataRow(GAME_DEV_SESH1_GUID_STR, "", (uint)0, SHORT_GENERIC_NOTES)]
    [DataRow(GAME_DEV_SESH2_GUID_STR, "", (uint)0, SHORT_GENERIC_NOTES)]
    [DataRow(GAME_DEV_SESH1_GUID_STR, "", (uint)60, SHORT_GENERIC_NOTES)]
    [DataRow(GAME_DEV_SESH1_GUID_STR, "", (uint)200, SHORT_GENERIC_NOTES)]
    [DataRow(GAME_DEV_SESH1_GUID_STR, "", (uint)999999999, SHORT_GENERIC_NOTES)]
    [DataRow(GAME_DEV_SESH1_GUID_STR, "2022-07-15T01:27:26Z", (uint)200, SHORT_GENERIC_NOTES)]
    [DataRow(GAME_DEV_SESH1_GUID_STR, MIN_DATE_STR, (uint)200, SHORT_GENERIC_NOTES)]
    [DataRow(GAME_DEV_SESH1_GUID_STR, "2025-07-15T01:27:26Z", (uint)200, SHORT_GENERIC_NOTES)]
    [TestCategory("UpdateSessionAsync")]
    public async Task UpdateSessionAsync_Admin_AnothersSession_Ok(
        string sessionIdStr,
        string startDateUtcStr,
        uint durationSeconds,
        string notes)
    {
        // -- Arrange --
        Guid.TryParse(sessionIdStr, out var sessionGuid);
        DateTime.TryParse(startDateUtcStr, out var startDateUtc);

        var updateSessionDto = new SessionUpdateDto
        {
            StartDateUtc = startDateUtc,
            DurationSeconds = durationSeconds,
            Notes = notes
        };

        var sessionService = _createSessionService();

        // -- Act --
        var session = await sessionService.UpdateSessionAsync(JANE_USER_GUID, sessionGuid, updateSessionDto);

        // -- Assert --
        // Check return object
        _assertReturnedFromUpdate(GAME_DEV_ACT_GUID, updateSessionDto, session);

        // Check the fake DB
        _assertDbFromUpdate(sessionGuid, GAME_DEV_ACT_GUID, updateSessionDto);
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
    [DataRow(GAME_DEV_SESH1_GUID_STR, "", (uint)0, SHORT_GENERIC_NOTES)]
    [DataRow(GAME_DEV_SESH2_GUID_STR, "", (uint)0, SHORT_GENERIC_NOTES)]
    [DataRow(GAME_DEV_SESH1_GUID_STR, "", (uint)60, SHORT_GENERIC_NOTES)]
    [DataRow(GAME_DEV_SESH1_GUID_STR, "", (uint)200, SHORT_GENERIC_NOTES)]
    [DataRow(GAME_DEV_SESH1_GUID_STR, "", (uint)999999999, SHORT_GENERIC_NOTES)]
    [DataRow(GAME_DEV_SESH1_GUID_STR, "2022-07-15T01:27:26Z", (uint)200, SHORT_GENERIC_NOTES)]
    [DataRow(GAME_DEV_SESH1_GUID_STR, MIN_DATE_STR, (uint)200, SHORT_GENERIC_NOTES)]
    [DataRow(GAME_DEV_SESH1_GUID_STR, "2025-07-15T01:27:26Z", (uint)200, SHORT_GENERIC_NOTES)]
    [TestCategory("UpdateSessionAsync")]
    public async Task UpdateSessionAsync_NonAdmin_OwnSession_Ok(
        string sessionIdStr,
        string startDateUtcStr,
        uint durationSeconds,
        string notes)
    {
        // -- Arrange --
        Guid.TryParse(sessionIdStr, out var sessionGuid);
        DateTime.TryParse(startDateUtcStr, out var startDateUtc);

        var updateSessionDto = new SessionUpdateDto
        {
            StartDateUtc = startDateUtc,
            DurationSeconds = durationSeconds,
            Notes = notes
        };

        var sessionService = _createSessionService();

        // -- Act --
        var session = await sessionService.UpdateSessionAsync(JOHN_USER_GUID, sessionGuid, updateSessionDto);

        // -- Assert --
        // Check return object
        _assertReturnedFromUpdate(GAME_DEV_ACT_GUID, updateSessionDto, session);

        // Check the fake DB
        _assertDbFromUpdate(sessionGuid, GAME_DEV_ACT_GUID, updateSessionDto);
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

    private void _assertSessionsEqual(Session expectedSession, SessionGetDto returnedSession)
    {
        Assert.IsNotNull(returnedSession);
        Assert.AreEqual(expectedSession.Id, returnedSession.Id);
        Assert.AreEqual(expectedSession.ActivityId, returnedSession.ActivityId);
        DatesEqualWithinSeconds((DateTime)expectedSession.StartDateUtc, returnedSession.StartDateUtc);
        Assert.AreEqual(expectedSession.DurationSeconds, returnedSession.DurationSeconds);
        Assert.AreEqual(expectedSession.Notes, returnedSession.Notes);
    }

    private void _assertReturnedFromCreate(SessionCreateDto createDto, SessionGetDto returnedSession)
    {
        Assert.IsNotNull(returnedSession);
        Assert.IsNotNull(returnedSession.Id);
        Assert.AreNotEqual(Guid.Empty, returnedSession.Id);
        Assert.AreEqual(createDto.ActivityId, returnedSession.ActivityId);
        DatesEqualWithinSeconds((DateTime)createDto.StartDateUtc, (DateTime)returnedSession.StartDateUtc);
        Assert.AreEqual(createDto.DurationSeconds, returnedSession.DurationSeconds);
        Assert.AreEqual(createDto.Notes, returnedSession.Notes);
    }

    private void _assertDbFromCreate(SessionCreateDto createDto)
    {
        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Once);
        Assert.AreEqual(6, allSessions.Count());
        var savedSesh = allSessions.Last();
        Assert.IsNotNull(savedSesh.Id);
        Assert.AreNotEqual(Guid.Empty, savedSesh.Id);
        Assert.AreEqual(createDto.ActivityId, savedSesh.ActivityId);
        DatesEqualWithinSeconds((DateTime)createDto.StartDateUtc, (DateTime)savedSesh.StartDateUtc);
        Assert.AreEqual(createDto.DurationSeconds, savedSesh.DurationSeconds);
        Assert.AreEqual(createDto.Notes, savedSesh.Notes);
    }

    private void _assertReturnedFromUpdate(Guid activityId, SessionUpdateDto updateDto, SessionGetDto returnedSession)
    {
        Assert.IsNotNull(returnedSession);
        Assert.IsNotNull(returnedSession.Id);
        Assert.AreNotEqual(Guid.Empty, returnedSession.Id);
        Assert.AreEqual(activityId, returnedSession.ActivityId);
        DatesEqualWithinSeconds((DateTime)updateDto.StartDateUtc, (DateTime)returnedSession.StartDateUtc);
        Assert.AreEqual(updateDto.DurationSeconds, returnedSession.DurationSeconds);
        Assert.AreEqual(updateDto.Notes, returnedSession.Notes);
    }

    private void _assertDbFromUpdate(Guid sessionId, Guid activityId, SessionUpdateDto updateDto)
    {
        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Once);
        Assert.AreEqual(5, allSessions.Count());
        var savedSesh = allSessions.First(X => X.Id == sessionId);
        Assert.IsNotNull(savedSesh.Id);
        Assert.AreNotEqual(Guid.Empty, savedSesh.Id);
        Assert.AreEqual(activityId, savedSesh.ActivityId);
        DatesEqualWithinSeconds((DateTime)updateDto.StartDateUtc, (DateTime)savedSesh.StartDateUtc);
        Assert.AreEqual(updateDto.DurationSeconds, savedSesh.DurationSeconds);
        Assert.AreEqual(updateDto.Notes, savedSesh.Notes);
    }
}
