using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Entities;
using ActivityTrackerApp.Exceptions;
using ActivityTrackerApp.Services;

using Moq;

using static ActivityTrackerAppTests.Fixtures.TestFixtures;
using static ActivityTrackerAppTests.Helpers.TestHelpers;

namespace ActivityTrackerAppTests;

// TODO: Need to test diff data edge cases
[TestClass]
public class SessionServiceTests : ServiceTestsBase
{
    // Called before all tests
    [ClassInitialize()]
    public static void InitializeClass(TestContext context)
    {
        // NOTE: We just call this base method in the child classes
        //       since [ClassInitialize] method can't be inherited
        initializeClass();
    }

    #region GetAllSessionsAsync
    [TestMethod]
    [TestCategory(nameof(SessionService.GetAllSessionsByActivityIdAsync))]
    public async Task GetAllSessionsByActivityIdAsync_Admin_AnothersSessions_Ok()
    {
        // -- Arrange --
        var sessionService = _createSessionService();

        // -- Act --
        var sessions = await sessionService.GetAllSessionsByActivityIdAsync(JANE_USER_GUID, GAME_DEV_ACT_GUID);

        // -- Assert --
        _assertGetAllOk(sessions);
    }

    [TestMethod]
    [TestCategory(nameof(SessionService.GetAllSessionsByActivityIdAsync))]
    public async Task GetAllSessionsByActivityIdAsync_NonAdmin_OwnSessions_Ok()
    {
        // -- Arrange --
        var sessionService = _createSessionService();

        // -- Act --
        var sessions = await sessionService.GetAllSessionsByActivityIdAsync(JOHN_USER_GUID, GAME_DEV_ACT_GUID);

        // -- Assert --
        _assertGetAllOk(sessions);
    }

    [TestMethod]
    [TestCategory(nameof(SessionService.GetAllSessionsByActivityIdAsync))]
    [TestCategory(nameof(ForbiddenException))]
    [ExpectedException(typeof(ForbiddenException))]
    public async Task GetAllSessionsByActivityIdAsync_NonAdmin_AnothersSessions_ThrowForbidden()
    {
        // -- Arrange --
        var sessionService = _createSessionService();

        // -- Act --
        var sessions = await sessionService.GetAllSessionsByActivityIdAsync(JOHN_USER_GUID, PANIC_ACT_GUID);
    }
    #endregion

    #region GetSessionAsync
    [TestMethod]
    [TestCategory(nameof(SessionService.GetSessionAsync))]
    public async Task GetSessionAsync_Admin_AnothersSession_Ok()
    {
        // -- Arrange --
        var sessionService = _createSessionService();

        // -- Act --
        var session = await sessionService.GetSessionAsync(JANE_USER_GUID, GAME_DEV_SESH1_GUID);

        // -- Assert --
        _assertGetOk(gameDevSesh1, session);
    }

    [TestMethod]
    [TestCategory(nameof(SessionService.GetSessionAsync))]
    public async Task GetSessionAsync_NonAdmin_OwnSession_Ok()
    {
        // -- Arrange --
        var sessionService = _createSessionService();

        // -- Act --
        var session = await sessionService.GetSessionAsync(JOHN_USER_GUID, GAME_DEV_SESH1_GUID);

        // -- Assert --
        _assertGetOk(gameDevSesh1, session);
    }

    [TestMethod]
    [TestCategory(nameof(SessionService.GetSessionAsync))]
    public async Task GetSessionAsync_NonAdmin_NonExistentSession_ReturnNull()
    {
        // -- Arrange --
        Guid.TryParse(NON_EXISTENT_GUID_STR, out var sessionGuid);
        var sessionService = _createSessionService();

        // -- Act --
        var session = await sessionService.GetSessionAsync(JOHN_USER_GUID, sessionGuid);

        // -- Assert --
        _assertGetReturnNull(session);
    }

    [TestMethod]
    [TestCategory(nameof(SessionService.GetSessionAsync))]
    public async Task GetSessionAsync_NonAdmin_DeletedSession_ReturnNull()
    {
        // -- Arrange --
        var sessionService = _createSessionService();

        // -- Act --
        var session = await sessionService.GetSessionAsync(JOHN_USER_GUID, GAME_DEV_SESH_DELETED_GUID);

        // -- Assert --
        _assertGetReturnNull(session);
    }

    [TestMethod]
    [TestCategory(nameof(SessionService.GetSessionAsync))]
    [TestCategory(nameof(ForbiddenException))]
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
    [TestCategory(nameof(SessionService.CreateSessionAsync))]
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
        _assertCreateOk(newSessionDto, session);
    }

    [TestMethod]
    [DataRow(GAME_DEV_ACT_GUID_STR, "", (uint)0, "")]
    [DataRow(GAME_DEV_ACT_GUID_STR, "", (uint)60, SHORT_GENERIC_NOTES)]
    [DataRow(GAME_DEV_ACT_GUID_STR, "", (uint)200, SHORT_GENERIC_NOTES)]
    [DataRow(GAME_DEV_ACT_GUID_STR, "", (uint)999999999, SHORT_GENERIC_NOTES)]
    [DataRow(GAME_DEV_ACT_GUID_STR, "2022-07-15T01:27:26Z", (uint)200, SHORT_GENERIC_NOTES)]
    [DataRow(GAME_DEV_ACT_GUID_STR, MIN_DATE_STR, (uint)200, SHORT_GENERIC_NOTES)]
    [DataRow(GAME_DEV_ACT_GUID_STR, "2025-07-15T01:27:26Z", (uint)200, SHORT_GENERIC_NOTES)]
    [TestCategory(nameof(SessionService.CreateSessionAsync))]
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
        _assertCreateOk(newSessionDto, session);
    }

    [TestMethod]
    [DataRow(NON_EXISTENT_GUID_STR, MIN_DATE_STR, (uint)200, SHORT_GENERIC_NOTES)]
    [TestCategory(nameof(SessionService.CreateSessionAsync))]
    public async Task CreateSessionAsync_NonAdmin_OwnSession_NonExistentSession_ReturnNull(
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

        var sessionService = _createSessionService();

        // -- Act --
        var session = await sessionService.CreateSessionAsync(JOHN_USER_GUID, newSessionDto);

        // -- Assert --
        _assertCreateReturnNull(session);
    }

    [TestMethod]
    [DataRow(GAME_DEV_SESH_DELETED_GUID_STR, MIN_DATE_STR, (uint)200, SHORT_GENERIC_NOTES)]
    [TestCategory(nameof(SessionService.CreateSessionAsync))]
    public async Task CreateSessionAsync_NonAdmin_OwnSession_DeletedSession_ReturnNull(
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

        var sessionService = _createSessionService();

        // -- Act --
        var session = await sessionService.CreateSessionAsync(JOHN_USER_GUID, newSessionDto);

        // -- Assert --
        _assertCreateReturnNull(session);
    }

    [TestMethod]
    [TestCategory(nameof(SessionService.CreateSessionAsync))]
    [TestCategory(nameof(ForbiddenException))]
    [ExpectedException(typeof(ForbiddenException))]
    public async Task CreateSessionAsync_NonAdmin_AnothersSession_ThrowForbidden()
    {
        // -- Arrange --
        var newSessionDto = new SessionCreateDto
        {
            ActivityId = PANIC_ACT_GUID
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
    [TestCategory(nameof(SessionService.UpdateSessionAsync))]
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
        _assertUpdateOk(sessionGuid, GAME_DEV_ACT_GUID, updateSessionDto, session);
    }

    [TestMethod]
    [TestCategory(nameof(SessionService.UpdateSessionAsync))]
    [TestCategory(nameof(ForbiddenException))]
    [ExpectedException(typeof(ForbiddenException))]
    public async Task UpdateSessionAsync_NonAdmin_AnothersSession_ThrowForbidden()
    {
        // -- Arrange --
        var updateSessionDto = new SessionUpdateDto();

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
    [TestCategory(nameof(SessionService.UpdateSessionAsync))]
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
        _assertUpdateOk(sessionGuid, GAME_DEV_ACT_GUID, updateSessionDto, session);
    }

    [TestMethod]
    [DataRow(NON_EXISTENT_GUID_STR, "2022-07-15T01:27:26Z", (uint)200, SHORT_GENERIC_NOTES)]
    [TestCategory(nameof(SessionService.UpdateSessionAsync))]
    public async Task UpdateSessionAsync_NonAdmin_OwnSession_NonExistentSession_ReturnNull(
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
        _assertUpdateReturnNull(session);
    }

    [TestMethod]
    [DataRow(GAME_DEV_SESH_DELETED_GUID_STR, "2022-07-15T01:27:26Z", (uint)200, SHORT_GENERIC_NOTES)]
    [TestCategory(nameof(SessionService.UpdateSessionAsync))]
    public async Task UpdateSessionAsync_NonAdmin_OwnSession_DeletedSession_ReturnNull(
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
        _assertUpdateReturnNull(session);
    }

    // TODO add more tests
    #endregion UpdateSessionAsync

    #region DeleteSessionAsync
    [TestMethod]
    [TestCategory(nameof(SessionService.DeleteSessionAsync))]
    public async Task DeleteSessionAsync_Admin_AnothersSession_Ok()
    {
        // -- Arrange --
        var sessionService = _createSessionService();

        // -- Act --
        var isSuccessful = await sessionService.DeleteSessionAsync(JANE_USER_GUID, GAME_DEV_SESH1_GUID);

        // -- Assert --
        _assertDeleteOk(isSuccessful);
    }

    [TestMethod]
    [TestCategory(nameof(SessionService.DeleteSessionAsync))]
    [TestCategory(nameof(ForbiddenException))]
    [ExpectedException(typeof(ForbiddenException))]
    public async Task DeleteSessionAsync_NonAdmin_AnothersSession_ThrowForbidden()
    {
        // -- Arrange --
        var sessionService = _createSessionService();

        // -- Act --
        var isSuccessful = await sessionService.DeleteSessionAsync(JOHN_USER_GUID, PANIC_SESH_GUID);
    }

    [TestMethod]
    [TestCategory(nameof(SessionService.DeleteSessionAsync))]
    public async Task DeleteSessionAsync_NonAdmin_OwnActivity_Ok()
    {
        // -- Arrange --
        var sessionService = _createSessionService();

        // -- Act --
        var isSuccessful = await sessionService.DeleteSessionAsync(JOHN_USER_GUID, GAME_DEV_SESH1_GUID);

        // -- Assert --
        _assertDeleteOk(isSuccessful);
    }

    [TestMethod]
    [TestCategory(nameof(SessionService.DeleteSessionAsync))]
    public async Task DeleteSessionAsync_NonAdmin_OwnSession_NonExistentSession_ReturnFalse()
    {
        // -- Arrange --
        var sessionService = _createSessionService();

        // -- Act --
        var isSuccessful = await sessionService.DeleteSessionAsync(JOHN_USER_GUID, NON_EXISTENT_GUID);

        // -- Assert --
        _assertDeleteReturnFalse(isSuccessful);
    }

    [TestMethod]
    [TestCategory(nameof(SessionService.DeleteSessionAsync))]
    public async Task DeleteSessionAsync_NonAdmin_OwnSession_DeletedSession_ReturnFalse()
    {
        // -- Arrange --
        var sessionService = _createSessionService();

        // -- Act --
        var isSuccessful = await sessionService.DeleteSessionAsync(JOHN_USER_GUID, GAME_DEV_SESH_DELETED_GUID);

        // -- Assert --
        _assertDeleteReturnFalse(isSuccessful);
    }
    #endregion DeleteSessionAsync

    private SessionService _createSessionService()
    {
        return new SessionService(
            dbContextMock.Object,
            userServiceMock.Object,
            mapperMock.Object);
    }

    private void _assertGetAllOk(IEnumerable<SessionGetDto> returnedSessions)
    {
        Assert.IsNotNull(returnedSessions);

        // Only non-deleted sessions
        Assert.AreEqual(2, returnedSessions.Count());
        var sessionsList = returnedSessions.ToList();
        _assertSessionsEqual(gameDevSesh1, sessionsList[0]);
        _assertSessionsEqual(gameDevSesh2, sessionsList[1]);

        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Never);
    }

    private void _assertGetOk(Session expectedSession, SessionGetDto returnedSession)
    {
        Assert.IsNotNull(returnedSession);
        _assertSessionsEqual(expectedSession, returnedSession);
        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Never);
    }

    private void _assertGetReturnNull(SessionGetDto returnedSession)
    {
        Assert.IsNull(returnedSession);
        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Never);
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

    private void _assertCreateOk(SessionCreateDto createDto, SessionGetDto returnedSession)
    {
        // Check return object
        _assertReturnedFromCreate(createDto, returnedSession);

        // Check the fake DB
        _assertDbFromCreate(createDto);
    }

    private void _assertCreateReturnNull(SessionGetDto returnedSession)
    {
        Assert.IsNull(returnedSession);
        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Never);
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

    private void _assertUpdateOk(Guid sessionId, Guid activityId, SessionUpdateDto updateDto, SessionGetDto returnedSession)
    {
        // Check return object
        _assertReturnedFromUpdate(activityId, updateDto, returnedSession);

        // Check the fake DB
        _assertDbFromUpdate(sessionId, activityId, updateDto);
    }

    private void _assertUpdateReturnNull(SessionGetDto returnedSession)
    {
        Assert.IsNull(returnedSession);
        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Never);
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

    private void _assertDeleteOk(bool isSuccessful)
    {
        Assert.IsTrue(isSuccessful);
        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Once);

        // Session should be soft-deleted
        Assert.IsNotNull(gameDevSesh1.DeletedDateUtc);
        Assert.IsTrue(DatesEqualWithinSeconds((DateTime)gameDevSesh1.DeletedDateUtc, DateTime.UtcNow));

        // The session's activity should not be soft-deleted
        Assert.IsNull(gameDevAct.DeletedDateUtc);
    }

    private void _assertDeleteReturnFalse(bool isSuccessful)
    {
        Assert.IsFalse(isSuccessful);
        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Never);
    }
}
