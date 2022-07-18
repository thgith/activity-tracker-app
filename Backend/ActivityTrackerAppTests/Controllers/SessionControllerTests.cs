using ActivityTrackerApp.Controllers;
using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Entities;
using ActivityTrackerApp.Exceptions;
using ActivityTrackerApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

using static ActivityTrackerAppTests.Fixtures.TestFixtures;
using static ActivityTrackerAppTests.Helpers.TestHelpers;

namespace ActivityTrackerAppTests;

// TODO: write these tests.
// Go to services to see tests that are actually written.
// Here will be similar but also need to set up the Request object.
[TestClass]
public class SessionControllerTests : ControllerTestsBase<SessionController>
{
    // Called before all tests
    [ClassInitialize()]
    public static void InitializeClass(TestContext context)
    {
        // NOTE: We just call this base method in the child classes
        //       since [ClassInitialize] method can't be inherited
        initializeClass();
    }

    #region GetAllSessionsByActivityIdAsync
    [TestMethod]
    [TestCategory(nameof(SessionService.GetAllSessionsByActivityIdAsync))]
    public async Task GetAllSessionsByActivityIdAsync_Authorized_Ok()
    {
        throw new NotImplementedException();
    }
    #endregion GetAllSessionsByActivityIdAsync

    #region GetSessionAsync
    [TestMethod]
    [TestCategory(nameof(SessionService.GetSessionAsync))]
    public async Task GetSessionAsync_Authorized_Ok()
    {
        throw new NotImplementedException();
    }
    #endregion GetSessionAsync

    #region CreateSessionAsync
    [TestMethod]
    [TestCategory(nameof(SessionService.CreateSessionAsync))]
    public async Task CreateSessionAsync_Authorized_Ok()
    {
        throw new NotImplementedException();
    }
    #endregion CreateSessionAsync

    #region UpdateSessionAsync
    [TestMethod]
    [TestCategory(nameof(SessionService.UpdateSessionAsync))]
    public async Task UpdateSessionAsync_Authorized_Ok()
    {
        throw new NotImplementedException();
    }
    #endregion UpdateSessionAsync

    #region DeleteSessionAsync
    [TestMethod]
    [TestCategory(nameof(SessionService.DeleteSessionAsync))]
    public async Task DeleteSessionAsync_Authorized_Ok()
    {
        throw new NotImplementedException();
    }
    #endregion DeleteSessionAsync

    private SessionController _createSessionCtrl()
    {
        return new SessionController(
            sessionServiceMock.Object,
            userServiceMock.Object,
            jwtServiceMock.Object,
            loggerMock.Object
        );
    }
}