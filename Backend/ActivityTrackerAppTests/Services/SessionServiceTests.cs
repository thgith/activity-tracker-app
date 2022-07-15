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
    // Called before all tests
    // TODO figure out why it didn't like being in the base class
    [ClassInitialize()]
    public static void InitializeClass(TestContext context)
    {
        // Init users here since they won't change through each activity test
        _usersData = new List<User> { GenerateJaneUser(), GenerateJohnUser(), GenerateJudyUser() };
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
    [TestMethod]

    public async Task DeleteActivityAsync_Admin_AnothersSession_Ok()
    {
        // -- Arrange --
        _userServiceMock.Setup(m => m.IsAdmin(JANE_USER_GUID))
                            .Returns(Task.FromResult(true));

        var sessionService = new SessionService(
            _dbContextMock.Object,
            _userServiceMock.Object,
            _mapperMock.Object);

        // -- Act --
        var isSuccessful = await sessionService.DeleteSessionAsync(JANE_USER_GUID, GAME_DEV_SESH1_GUID);

        // -- Assert --
        Assert.IsTrue(isSuccessful);
        _dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Once);

        // Sessions should be soft-deleted
        Assert.IsNotNull(_gameDevSesh1.DeletedDateUtc);
        Assert.IsTrue(DatesEqualWithinSeconds((DateTime)_gameDevSesh1.DeletedDateUtc, DateTime.UtcNow));
    }

    [TestMethod]
    [ExpectedException(typeof(ForbiddenException))]

    public async Task DeleteActivityAsync_NonAdmin_AnothersSession_ThrowForbidden()
    {
        // -- Arrange --
        _userServiceMock.Setup(m => m.IsAdmin(JOHN_USER_GUID))
                            .Returns(Task.FromResult(false));

        var sessionService = new SessionService(
            _dbContextMock.Object,
            _userServiceMock.Object,
            _mapperMock.Object);

        // -- Act --
        var isSuccessful = await sessionService.DeleteSessionAsync(JOHN_USER_GUID, PANIC_SESH_GUID);
    }

    [TestMethod]

    public async Task DeleteActivityAsync_NonAdmin_OwnActivity_Ok()
    {
        // -- Arrange --
        _userServiceMock.Setup(m => m.IsAdmin(JOHN_USER_GUID))
                            .Returns(Task.FromResult(false));

        var sessionService = new SessionService(
            _dbContextMock.Object,
            _userServiceMock.Object,
            _mapperMock.Object);

        // -- Act --
        var isSuccessful = await sessionService.DeleteSessionAsync(JOHN_USER_GUID, GAME_DEV_SESH1_GUID);

        // -- Assert --
        Assert.IsTrue(isSuccessful);
        _dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Once);

        // Sessions should be soft-deleted
        Assert.IsNotNull(_gameDevSesh1.DeletedDateUtc);
        Assert.IsTrue(DatesEqualWithinSeconds((DateTime)_gameDevSesh1.DeletedDateUtc, DateTime.UtcNow));
    }
    #endregion DeleteSessionAsync
}
