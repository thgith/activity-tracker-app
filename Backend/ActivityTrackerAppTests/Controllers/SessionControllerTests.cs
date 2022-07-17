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

// TODO: write these tests
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