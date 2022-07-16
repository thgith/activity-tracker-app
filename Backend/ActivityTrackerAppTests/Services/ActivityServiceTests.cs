using ActivityTrackerApp.Entities;
using ActivityTrackerApp.Services;
using static ActivityTrackerAppTests.Fixtures.TestFixtures;
using static ActivityTrackerAppTests.Helpers.TestHelpers;
using Moq;
using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Exceptions;

namespace ActivityTrackerAppTests;

// TODO: Need to test diff data edge cases
// NOTE: Prob should add more checks to check side effects (call count, etc)
[TestClass]
public class ActivityServiceTests : TestBase
{
    private static Mock<ISessionService> _sessionsService;

    // Called before all tests
    // TODO figure out why it didn't like being in the base class
    [ClassInitialize()]
    public static void InitializeClass(TestContext context)
    {
        // Init users here since they won't change through each activity test
        usersData = new List<User> { GenerateJaneUser(), GenerateJohnUser(), GenerateJudyUser() };
    }

    #region GetAllActivitiesAsync
    [TestMethod]
    [TestCategory("GetAllActivitiesAsync")]
    public async Task GetAllActivitiesAsync_Admin_AnotherUser_Ok()
    {
        // -- Arrange --
        var activityService = _createActivityService();

        // -- Act --
        var activities = await activityService.GetAllActivitiesForUserAsync(JANE_USER_GUID, JOHN_USER_GUID);

        // -- Assert --
        Assert.IsNotNull(activities);
        Assert.AreEqual(activities.Count(), 3);
        var activitiesList = activities.ToList();

        // This list should be ordered by join date, so the activities should be in this order
        _assertActivitiesSame(activitiesList[0], gameDevAct);
        _assertActivitiesSame(activitiesList[1], pianoAct);
        _assertActivitiesSame(activitiesList[2], mcatAct);

        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Never);
    }

    [TestMethod]
    [TestCategory("GetAllActivitiesAsync")]
    public async Task GetAllActivitiesAsync_NonAdmin_OwnActivities_Ok()
    {
        // -- Arrange --
        var activityService = _createActivityService();

        // -- Act --
        var activities = await activityService.GetAllActivitiesForUserAsync(JOHN_USER_GUID, JOHN_USER_GUID);

        // -- Assert --
        Assert.IsNotNull(activities);
        Assert.AreEqual(activities.Count(), 3);
        var activitiesList = activities.ToList();

        // This list should be ordered by join date, so the activities should be in this order
        _assertActivitiesSame(activitiesList[0], gameDevAct);
        _assertActivitiesSame(activitiesList[1], pianoAct);
        _assertActivitiesSame(activitiesList[2], mcatAct);

        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Never);
    }

    [TestMethod]
    [TestCategory("GetActivityAsync")]
    [TestCategory("Forbidden")]
    [ExpectedException(typeof(ForbiddenException))]
    public async Task GetAllActivitiesAsync_NonAdmin_AnothersActivities_ThrowForbidden()
    {
        // -- Arrange --
        var activityService = _createActivityService();

        // -- Act --
        var activities = await activityService.GetAllActivitiesForUserAsync(JOHN_USER_GUID, JANE_USER_GUID);
    }

    [TestMethod]
    [TestCategory("GetActivityAsync")]
    public async Task GetAllActivitiesAsync_Admin_AnothersActivity_DeletedActivity_ReturnEmpty()
    {
        // -- Arrange --
        var activityService = _createActivityService();

        // -- Act --
        var activities = await activityService.GetAllActivitiesForUserAsync(JANE_USER_GUID, JUDY_USER_GUID);

        // -- Assert --
        Assert.AreEqual(activities.Count(), 0);
    }

    #endregion

    #region GetActivityAsync
    [TestMethod]
    [DataRow(GAME_DEV_ACT_GUID_STR)]
    [DataRow(PIANO_ACT_GUID_STR)]
    [DataRow(MCAT_ACT_GUID_STR)]
    [TestCategory("GetActivityAsync")]
    public async Task GetActivityAsync_Admin_AnothersActivity_Ok(string activityIdStr)
    {
        // -- Arrange --
        var activityService = _createActivityService();

        // -- Act --
        Guid.TryParse(activityIdStr, out var activityGuid);
        var activity = await activityService.GetActivityAsync(JANE_USER_GUID, activityGuid);

        // -- Assert --
        Assert.IsNotNull(activity);
        _assertActivitiesSame(activity, allActs.First(x => x.Id == activityGuid));
    }

    [TestMethod]
    [DataRow(GAME_DEV_ACT_GUID_STR)]
    [DataRow(PIANO_ACT_GUID_STR)]
    [DataRow(MCAT_ACT_GUID_STR)]
    [TestCategory("GetActivityAsync")]
    public async Task GetActivityAsync_NonAdmin_OwnActivity_Ok(string activityIdStr)
    {
        // -- Arrange --
        var activityService = _createActivityService();

        // -- Act --
        Guid.TryParse(activityIdStr, out var activityGuid);
        var activity = await activityService.GetActivityAsync(JOHN_USER_GUID, activityGuid);

        // -- Assert --
        Assert.IsNotNull(activity);
        _assertActivitiesSame(activity, allActs.First(x => x.Id == activityGuid));
        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Never);
    }

    [TestMethod]
    [TestCategory("GetActivityAsync")]
    [TestCategory("Forbidden")]
    [ExpectedException(typeof(ForbiddenException))]
    public async Task GetActivityAsync_NonAdmin_AnothersActivity_ThrowForbidden()
    {
        // -- Arrange --
        var activityService = _createActivityService();

        // -- Act --
        var activity = await activityService.GetActivityAsync(JOHN_USER_GUID, PANIC_ACT_GUID);
    }

    [TestMethod]
    [TestCategory("GetActivityAsync")]
    public async Task GetActivityAsync_Admin_AnothersActivity_DeletedActivity_ReturnEmpty()
    {
        // -- Arrange --
        var activityService = _createActivityService();

        // -- Act --
        var activity = await activityService.GetActivityAsync(JANE_USER_GUID, SLEEPING_ACT_GUID);

        // -- Assert --
        Assert.IsNull(activity);
        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Never);
    }

    #endregion GetActivityAsync

    #region CreateActivityAsync

    [TestMethod]
    [DataRow("NEW NAME", "NEW DESC", "2022-07-12T01:27:26Z", "2022-07-13T01:27:26Z", "2022-07-14T01:27:26Z", "#3366ff")]
    [TestCategory("CreateActivityAsync")]
    public async Task CreateActivityAsync_Admin_AnothersActivity_Ok(
        string name,
        string description,
        string startDateUtcStr,
        string dueDateUtcStr,
        string completedDateUtcStr,
        string colorHex)
    {
        // -- Arrange --
        DateTime.TryParse(startDateUtcStr, out var startDateUtc);
        DateTime.TryParse(dueDateUtcStr, out var dueDateUtc);
        DateTime.TryParse(completedDateUtcStr, out var completedDateUtc);

        var actCreateDto = new ActivityCreateDto
        {
            Name = name,
            Description = description,
            StartDateUtc = startDateUtc,
            DueDateUtc = dueDateUtc,
            CompletedDateUtc = completedDateUtc,
            ColorHex = colorHex,
            Tags = null
        };

        var activityService = _createActivityService();

        // -- Act --
        var activity = await activityService.CreateActivityAsync(JANE_USER_GUID, JOHN_USER_GUID, actCreateDto);

        // -- Assert --
        // Verify returned object is as expected
        _verifyReturnedFromCreate(actCreateDto, activity);

        // Verify DB changed
        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Once);
    }

    [TestMethod]
    [TestCategory("CreateActivityAsync")]
    [TestCategory("Forbidden")]
    [ExpectedException(typeof(ForbiddenException))]
    public async Task CreateActivityAsync_NonAdmin_AnothersActivity_ThrowForbidden()
    {
        // -- Arrange --
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

        var activityService = _createActivityService();

        // -- Act --
        var activity = await activityService.CreateActivityAsync(JOHN_USER_GUID, JANE_USER_GUID, actCreateDto);
    }

    [TestMethod]
    [DataRow("NEW NAME", "NEW DESC", "2022-07-12T01:27:26Z", "2022-07-12T01:27:26Z", "2022-07-12T01:27:26Z", "#3366ff")]
    [DataRow("NEW NAME", "NEW DESC", "2022-07-12T01:27:26Z", "2022-07-13T01:27:26Z", "2022-07-14T01:27:26Z", "#3366ff")]
    [DataRow("NEW NAME", "NEW DESC", "2022-07-12T01:27:26Z", null, "2022-07-14T01:27:26Z", "#3366ff")]
    [DataRow("NEW NAME", "NEW DESC", "2022-07-12T01:27:26Z", "2022-07-13T01:27:26Z", null, "#3366ff")]
    [DataRow("NEW NAME", "NEW DESC", "2022-07-12T01:27:26Z", null, null, "#3366ff")]
    // StartDateUtc null so will be UtcNow.
    [DataRow("NEW NAME", "NEW DESC", null, null, null, "#3366ff")]
    [TestCategory("CreateActivityAsync")]
    public async Task CreateActivityAsync_NonAdmin_OwnActivity_Ok(
        string name,
        string description,
        string startDateUtcStr,
        string dueDateUtcStr,
        string completedDateUtcStr,
        string colorHex)
    {
        // -- Arrange --
        DateTime.TryParse(startDateUtcStr, out var startDateUtc);
        DateTime.TryParse(dueDateUtcStr, out var dueDateUtc);
        DateTime.TryParse(completedDateUtcStr, out var completedDateUtc);

        var actCreateDto = new ActivityCreateDto
        {
            Name = name,
            Description = description,
            StartDateUtc = startDateUtc,
            DueDateUtc = dueDateUtc,
            CompletedDateUtc = completedDateUtc,
            ColorHex = colorHex,
            Tags = null
        };

        var activityService = _createActivityService();

        // -- Act --
        var activity = await activityService.CreateActivityAsync(JOHN_USER_GUID, JOHN_USER_GUID, actCreateDto);

        // -- Assert --
        // Verify returned object is as expected
        _verifyReturnedFromCreate(actCreateDto, activity);

        // Verify DB changed
        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Once);
    }

    [TestMethod]
    // StartDateUtc null so will be UtcNow. StartDateUtc is greater than DueDateUtc.
    [DataRow("NEW NAME", "NEW DESC", null, PREHISTORIC_DATE_STR, FUTURE_DATE_STR, "#3366ff")]
    // StartDateUtc null so will be UtcNow.  StartDateUtc is greater than CompletedDateUtc.
    [DataRow("NEW NAME", "NEW DESC", null, FUTURE_DATE_STR, PREHISTORIC_DATE_STR, "#3366ff")]
    // None null. StartDateUtc is greater than CompletedDateUtc.
    [DataRow("NEW NAME", "NEW DESC", "2023-07-13T01:27:26Z", FUTURE_DATE_STR, PREHISTORIC_DATE_STR, "#3366ff")]
    // CompletedDateUtc null. StartDateUtc is greater than DueDateUtc.
    [DataRow("NEW NAME", "NEW DESC", "2023-07-13T01:27:26Z", PREHISTORIC_DATE_STR, null, "#3366ff")]
    // DueDateUtc null. StartDateUtc is greater than CompletedDateUtc.
    [DataRow("NEW NAME", "NEW DESC", "2023-07-13T01:27:26Z", null, PREHISTORIC_DATE_STR, "#3366ff")]
    [TestCategory("CreateActivityAsync")]
    [TestCategory("InvalidData")]
    [ExpectedException(typeof(InvalidDataException))]
    public async Task CreateActivityAsync_NonAdmin_OwnActivity_ThrowInvalidData(
        string name,
        string description,
        string startDateUtcStr,
        string dueDateUtcStr,
        string completedDateUtcStr,
        string colorHex)
    {
        // -- Arrange --
        DateTime.TryParse(startDateUtcStr, out var startDateUtc);
        DateTime.TryParse(dueDateUtcStr, out var dueDateUtc);
        DateTime.TryParse(completedDateUtcStr, out var completedDateUtc);

        var actCreateDto = new ActivityCreateDto
        {
            Name = name,
            Description = description,
            StartDateUtc = startDateUtc,
            DueDateUtc = dueDateUtc,
            CompletedDateUtc = completedDateUtc,
            ColorHex = colorHex,
            Tags = null
        };

        var activityService = _createActivityService();

        // -- Act --
        var activity = await activityService.CreateActivityAsync(JOHN_USER_GUID, JOHN_USER_GUID, actCreateDto);

        // -- Assert --
        // Verify returned object is as expected
        _verifyReturnedFromCreate(actCreateDto, activity);

        // Verify DB changed
        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Once);
    }

    // TODO more tests
    #endregion CreateActivityAsync

    #region UpdateActivityAsync

    [TestMethod]
    [DataRow("UPDATED NAME", "UPDATE DESC", "2022-07-12T01:27:26Z", "2022-07-12T01:27:26Z", "2022-07-12T01:27:26Z", "#3366ff")]
    [DataRow("UPDATED NAME", "UPDATE DESC", "2022-07-12T01:27:26Z", "2022-07-13T01:27:26Z", "2022-07-14T01:27:26Z", "#3366ff")]
    [DataRow("UPDATED NAME", "UPDATE DESC", "2022-07-12T01:27:26Z", null, "2022-07-14T01:27:26Z", "#3366ff")]
    [DataRow("UPDATED NAME", "UPDATE DESC", "2022-07-12T01:27:26Z", "2022-07-13T01:27:26Z", null, "#3366ff")]
    [DataRow("UPDATED NAME", "UPDATE DESC", "2022-07-12T01:27:26Z", null, null, "#3366ff")]
    // StartDateUtc null so will be UtcNow.
    [DataRow("UPDATED NAME", "UPDATED DESC", null, null, null, "#3366ff")]
    [TestCategory("UpdateActivityAsync")]
    public async Task UpdateActivityAsync_Admin_AnothersActivity_Ok(
        string name,
        string description,
        string startDateUtcStr,
        string dueDateUtcStr,
        string completedDateUtcStr,
        string colorHex)
    {
        // -- Arrange --
        DateTime.TryParse(startDateUtcStr, out var startDateUtc);
        DateTime.TryParse(dueDateUtcStr, out var dueDateUtc);
        DateTime.TryParse(completedDateUtcStr, out var completedDateUtc);

        var actUpdateDto = new ActivityUpdateDto
        {
            Name = name,
            Description = description,
            StartDateUtc = startDateUtc,
            DueDateUtc = dueDateUtc,
            CompletedDateUtc = completedDateUtc,
            ColorHex = colorHex,
            Tags = null
        };

        var activityService = _createActivityService();

        // -- Act --
        var activity = await activityService.UpdateActivityAsync(JANE_USER_GUID, GAME_DEV_ACT_GUID, actUpdateDto);

        // -- Assert --
        // Verify the returned object is as expected
        _verifyReturnedFromUpdate(GAME_DEV_ACT_GUID, actUpdateDto, activity);

        // Verify the DB is changed
        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Once);
    }
    [TestMethod]
    [DataRow("UPDATED NAME", "UPDATE DESC", MIN_DATE_STR, null, null, "#3366ff")]
    [TestCategory("UpdateActivityAsync")]

    public async Task UpdateActivityAsync_NonAdmin_OwnActivity_StartDateMinDate_Ok(
        string name,
        string description,
        string startDateUtcStr,
        string dueDateUtcStr,
        string completedDateUtcStr,
        string colorHex)
    {
        // -- Arrange --
        DateTime.TryParse(startDateUtcStr, out var startDateUtc);
        DateTime.TryParse(dueDateUtcStr, out var dueDateUtc);
        DateTime.TryParse(completedDateUtcStr, out var completedDateUtc);

        var actUpdateDto = new ActivityUpdateDto
        {
            Name = name,
            Description = description,
            StartDateUtc = startDateUtc,
            DueDateUtc = dueDateUtc,
            CompletedDateUtc = completedDateUtc,
            ColorHex = colorHex,
            Tags = null
        };

        var activityService = _createActivityService();

        // -- Act --
        var activity = await activityService.UpdateActivityAsync(JOHN_USER_GUID, GAME_DEV_ACT_GUID, actUpdateDto);

        // -- Assert --
        // Verify the returned object is as expected
        _verifyReturnedFromUpdate(GAME_DEV_ACT_GUID, actUpdateDto, activity);

        // Verify the DB is changed
        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Once);
    }

    [TestMethod]
    [DataRow("UPDATED NAME", "UPDATE DESC", "2022-07-12T01:27:26Z", "2022-07-12T01:27:26Z", "2022-07-12T01:27:26Z", "#3366ff")]
    [DataRow("UPDATED NAME", "UPDATE DESC", "2022-07-12T01:27:26Z", "2022-07-13T01:27:26Z", "2022-07-14T01:27:26Z", "#3366ff")]
    [DataRow("UPDATED NAME", "UPDATE DESC", "2022-07-12T01:27:26Z", null, "2022-07-14T01:27:26Z", "#3366ff")]
    [DataRow("UPDATED NAME", "UPDATE DESC", "2022-07-12T01:27:26Z", "2022-07-13T01:27:26Z", null, "#3366ff")]
    [DataRow("UPDATED NAME", "UPDATE DESC", "2022-07-12T01:27:26Z", null, null, "#3366ff")]
    // StartDateUtc null so will be UtcNow.
    [DataRow("UPDATED NAME", "UPDATED DESC", null, null, null, "#3366ff")]
    [TestCategory("UpdateActivityAsync")]
    public async Task UpdateActivityAsync_NonAdmin_OwnActivity_Ok(
        string name,
        string description,
        string startDateUtcStr,
        string dueDateUtcStr,
        string completedDateUtcStr,
        string colorHex)
    {
        // -- Arrange --
        DateTime.TryParse(startDateUtcStr, out var startDateUtc);
        DateTime.TryParse(dueDateUtcStr, out var dueDateUtc);
        DateTime.TryParse(completedDateUtcStr, out var completedDateUtc);

        var actUpdateDto = new ActivityUpdateDto
        {
            Name = name,
            Description = description,
            StartDateUtc = startDateUtc,
            DueDateUtc = dueDateUtc,
            CompletedDateUtc = completedDateUtc,
            ColorHex = colorHex,
            Tags = null
        };

        var activityService = _createActivityService();

        // -- Act --
        var activity = await activityService.UpdateActivityAsync(JOHN_USER_GUID, GAME_DEV_ACT_GUID, actUpdateDto);

        // -- Assert --
        // Verify the returned object is as expected
        _verifyReturnedFromUpdate(GAME_DEV_ACT_GUID, actUpdateDto, activity);

        // Verify the DB is changed
        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Once);
    }

    [TestMethod]
    // StartDateUtc null so will be UtcNow. StartDateUtc is greater than DueDateUtc.
    [DataRow("NEW NAME", "NEW DESC", null, PREHISTORIC_DATE_STR, FUTURE_DATE_STR, "#3366ff")]
    // StartDateUtc null so will be UtcNow.  StartDateUtc is greater than CompletedDateUtc.
    [DataRow("NEW NAME", "NEW DESC", null, FUTURE_DATE_STR, PREHISTORIC_DATE_STR, "#3366ff")]
    // None null. StartDateUtc is greater than CompletedDateUtc.
    [DataRow("NEW NAME", "NEW DESC", "2023-07-13T01:27:26Z", FUTURE_DATE_STR, PREHISTORIC_DATE_STR, "#3366ff")]
    // CompletedDateUtc null. StartDateUtc is greater than DueDateUtc.
    [DataRow("NEW NAME", "NEW DESC", "2023-07-13T01:27:26Z", PREHISTORIC_DATE_STR, null, "#3366ff")]
    // DueDateUtc null. StartDateUtc is greater than CompletedDateUtc.
    [DataRow("NEW NAME", "NEW DESC", "2023-07-13T01:27:26Z", null, PREHISTORIC_DATE_STR, "#3366ff")]
    [TestCategory("UpdateActivityAsync")]
    [TestCategory("InvalidData")]
    [ExpectedException(typeof(InvalidDataException))]
    public async Task UpdateActivityAsync_NonAdmin_OwnActivity_ThrowInvalidData(
        string name,
        string description,
        string startDateUtcStr,
        string dueDateUtcStr,
        string completedDateUtcStr,
        string colorHex)
    {
        // -- Arrange --
        DateTime.TryParse(startDateUtcStr, out var startDateUtc);
        DateTime.TryParse(dueDateUtcStr, out var dueDateUtc);
        DateTime.TryParse(completedDateUtcStr, out var completedDateUtc);

        var actUpdateDto = new ActivityUpdateDto
        {
            Name = name,
            Description = description,
            StartDateUtc = startDateUtc,
            DueDateUtc = dueDateUtc,
            CompletedDateUtc = completedDateUtc,
            ColorHex = colorHex,
            Tags = null
        };

        var activityService = _createActivityService();

        // -- Act --
        var activity = await activityService.UpdateActivityAsync(JOHN_USER_GUID, GAME_DEV_ACT_GUID, actUpdateDto);

        // -- Assert --
        // Verify the returned object is as expected
        _verifyReturnedFromUpdate(GAME_DEV_ACT_GUID, actUpdateDto, activity);

        // Verify the DB is changed
        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Once);
    }

    [TestMethod]
    [TestCategory("UpdateActivityAsync")]
    [TestCategory("Forbidden")]
    [ExpectedException(typeof(ForbiddenException))]
    public async Task UpdateActivityAsync_NonAdmin_AnothersActivity_ThrowForbidden()
    {
        // -- Arrange --
        var actUpdateDto = new ActivityUpdateDto
        {
            Name = "UPDATE GAME ACT",
            Description = "A new activity",
            StartDateUtc = DateTime.UtcNow.AddDays(-3),
            DueDateUtc = DateTime.UtcNow.AddDays(-2),
            CompletedDateUtc = DateTime.UtcNow.AddDays(-1),
            ColorHex = "#3366ff",
            Tags = null
        };

        var activityService = _createActivityService();

        // -- Act --
        var activity = await activityService.UpdateActivityAsync(JOHN_USER_GUID, PANIC_ACT_GUID, actUpdateDto);
    }

    // TODO add more tests
    #endregion UpdateActivityAsync

    #region DeleteActivityAsync
    [TestMethod]
    [TestCategory("DeleteActivityAsync")]
    public async Task DeleteActivityAsync_Admin_AnothersActivity_Ok()
    {
        // -- Arrange --
        var activityService = _createActivityService();

        // -- Act --
        var isSuccessful = await activityService.DeleteActivityAsync(JANE_USER_GUID, GAME_DEV_ACT_GUID);

        // -- Assert --
        Assert.IsTrue(isSuccessful);
        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Once);

        // Activity should be soft-deleted
        Assert.IsNotNull(gameDevAct.DeletedDateUtc);
        Assert.IsTrue(DatesEqualWithinSeconds((DateTime)gameDevAct.DeletedDateUtc, DateTime.UtcNow));

        // Sessions should be deleted too
        Assert.IsNotNull(gameDevSesh1.DeletedDateUtc);
        Assert.IsNotNull(gameDevSesh2.DeletedDateUtc);
        Assert.IsTrue(DatesEqualWithinSeconds((DateTime)gameDevSesh1.DeletedDateUtc, DateTime.UtcNow));
        Assert.IsTrue(DatesEqualWithinSeconds((DateTime)gameDevSesh2.DeletedDateUtc, DateTime.UtcNow));
    }

    [TestMethod]
    [TestCategory("DeleteActivityAsync")]
    [TestCategory("Forbidden")]
    [ExpectedException(typeof(ForbiddenException))]

    public async Task DeleteActivityAsync_NonAdmin_AnothersActivity_ThrowForbidden()
    {
        // -- Arrange --
        var activityService = _createActivityService();

        // -- Act --
        var isSuccessful = await activityService.DeleteActivityAsync(JOHN_USER_GUID, PANIC_ACT_GUID);
    }

    [TestMethod]
    [TestCategory("DeleteActivityAsync")]

    public async Task DeleteActivityAsync_NonAdmin_OwnActivity_Ok()
    {
        // -- Arrange --
        var activityService = _createActivityService();

        // -- Act --
        var isSuccessful = await activityService.DeleteActivityAsync(JOHN_USER_GUID, GAME_DEV_ACT_GUID);

        // -- Assert --
        Assert.IsTrue(isSuccessful);
        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Once);

        // Activity should be soft-deleted
        Assert.IsNotNull(gameDevAct.DeletedDateUtc);
        Assert.IsTrue(DatesEqualWithinSeconds((DateTime)gameDevAct.DeletedDateUtc, DateTime.UtcNow));

        // Sessions should be deleted too
        Assert.IsNotNull(gameDevSesh1.DeletedDateUtc);
        Assert.IsNotNull(gameDevSesh2.DeletedDateUtc);
        Assert.IsTrue(DatesEqualWithinSeconds((DateTime)gameDevSesh1.DeletedDateUtc, DateTime.UtcNow));
        Assert.IsTrue(DatesEqualWithinSeconds((DateTime)gameDevSesh2.DeletedDateUtc, DateTime.UtcNow));
    }

    // TODO more tests
    #endregion DeleteActivityAsync

    private ActivityService _createActivityService()
    {
        return new ActivityService(
            dbContextMock.Object,
            userServiceMock.Object,
            sessionServiceMock.Object,
            mapperMock.Object);
    }

    private void _assertActivitiesSame(
        ActivityGetDto actualActivity,
        Activity expectedActivity)
    {
        Assert.IsTrue(actualActivity != null);
        Assert.AreEqual(expectedActivity.Id, actualActivity.Id);
        Assert.AreEqual(expectedActivity.OwnerId, actualActivity.OwnerId);
        Assert.AreEqual(expectedActivity.Name, actualActivity.Name);
        Assert.AreEqual(expectedActivity.Description, actualActivity.Description);
        Assert.AreEqual(expectedActivity.ColorHex, actualActivity.ColorHex);
        Assert.AreEqual(expectedActivity.IsArchived, actualActivity.IsArchived);

        if (expectedActivity.Tags == null)
        {
            Assert.IsNull(actualActivity.Tags);
        }
        else
        {
            Assert.IsNotNull(actualActivity.Tags);
            Assert.AreEqual(expectedActivity.Tags.Count(), actualActivity.Tags.Count());
            foreach (var tag in expectedActivity.Tags)
            {
                actualActivity.Tags.Contains(tag);
            }
            foreach (var tag in actualActivity.Tags)
            {
                expectedActivity.Tags.Contains(tag);
            }
        }

        // Check that the dates are equal within a minute
        Assert.IsTrue(DatesEqualWithinSeconds((DateTime)actualActivity.StartDateUtc, DateTime.UtcNow));
        if (actualActivity.DueDateUtc != null)
        {
            Assert.IsTrue(DatesEqualWithinSeconds((DateTime)actualActivity.DueDateUtc, DateTime.UtcNow));
        }
        if (actualActivity.CompletedDateUtc != null)
        {
            Assert.IsTrue(DatesEqualWithinSeconds((DateTime)actualActivity.CompletedDateUtc, DateTime.UtcNow, 60));
        }
    }

    private void _verifyReturnedFromCreate(ActivityCreateDto createDto, ActivityGetDto returnedActivity)
    {
        Assert.IsNotNull(returnedActivity);
        Assert.AreEqual(createDto.Name, returnedActivity.Name);
        Assert.AreEqual(createDto.Description, returnedActivity.Description);
        DatesEqualWithinSeconds((DateTime)createDto.StartDateUtc, (DateTime)returnedActivity.StartDateUtc);
        _checkDueDateUtc();
        _checkCompletedDateUtc();
        Assert.AreEqual(createDto.ColorHex, returnedActivity.ColorHex);
        Assert.IsFalse(returnedActivity.IsArchived);
        Assert.IsNull(returnedActivity.Tags);

        void _checkDueDateUtc()
        {
            if (createDto.DueDateUtc == null || createDto.DueDateUtc == DateTime.MinValue)
            {
                Assert.IsNull(returnedActivity.DueDateUtc);
            }
            else
            {
                Assert.IsNotNull(returnedActivity.DueDateUtc);
                DatesEqualWithinSeconds((DateTime)createDto.DueDateUtc, (DateTime)returnedActivity.DueDateUtc);
            }
        }
        void _checkCompletedDateUtc()
        {
            if (createDto.CompletedDateUtc == null || createDto.CompletedDateUtc == DateTime.MinValue)
            {
                Assert.IsNull(returnedActivity.CompletedDateUtc);
            }
            else
            {
                DatesEqualWithinSeconds((DateTime)createDto.CompletedDateUtc, (DateTime)returnedActivity.CompletedDateUtc);
            }
        }
    }

    private void _verifyReturnedFromUpdate(Guid activityId, ActivityUpdateDto updateDto, ActivityGetDto returnedActivity)
    {
        Assert.IsNotNull(returnedActivity);
        Assert.AreEqual(updateDto.Name, returnedActivity.Name);
        Assert.AreEqual(updateDto.Description, returnedActivity.Description);
        DatesEqualWithinSeconds((DateTime)updateDto.StartDateUtc, (DateTime)returnedActivity.StartDateUtc);
        _checkDueDateUtc();
        _checkCompletedDateUtc();

        Assert.AreEqual(updateDto.ColorHex, returnedActivity.ColorHex);
        Assert.IsFalse(returnedActivity.IsArchived);
        // Didn't change the tags here. TODO need to check when tags is null and tags is added and deleted
        Assert.AreEqual(allActs.First(x => x.Id == activityId).Tags.Count(), returnedActivity.Tags.Count());

        void _checkDueDateUtc()
        {
            if (updateDto.DueDateUtc == null || updateDto.DueDateUtc == DateTime.MinValue)
            {
                Assert.IsNull(returnedActivity.DueDateUtc);
            }
            else
            {
                Assert.IsNotNull(returnedActivity.DueDateUtc);
                DatesEqualWithinSeconds((DateTime)updateDto.DueDateUtc, (DateTime)returnedActivity.DueDateUtc);
            }
        }
        void _checkCompletedDateUtc()
        {
            if (updateDto.CompletedDateUtc == null || updateDto.CompletedDateUtc == DateTime.MinValue)
            {
                Assert.IsNull(returnedActivity.CompletedDateUtc);
            }
            else
            {
                DatesEqualWithinSeconds((DateTime)updateDto.CompletedDateUtc, (DateTime)returnedActivity.CompletedDateUtc);
            }
        }
    }
}