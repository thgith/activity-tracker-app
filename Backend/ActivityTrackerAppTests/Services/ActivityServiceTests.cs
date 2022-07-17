using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Entities;
using ActivityTrackerApp.Exceptions;
using ActivityTrackerApp.Services;

using Moq;

using static ActivityTrackerAppTests.Fixtures.TestFixtures;
using static ActivityTrackerAppTests.Helpers.TestHelpers;

namespace ActivityTrackerAppTests;

// TODO: Need to test diff data edge cases
// NOTE: Prob should add more checks to check side effects (call count, etc)
[TestClass]
public class ActivityServiceTests : ServiceTestsBase
{
    private static Mock<ISessionService> _sessionsServiceMock;
    
    // Called before all tests
    [ClassInitialize()]
    public static void InitializeClass(TestContext context)
    {
        // NOTE: We just call this base method in the child classes
        //       since [ClassInitialize] method can't be inherited
        initializeClass();
    }

    // Called before each test
    [TestInitialize()]
    public void InitializeActivityTests()
    {
        _sessionsServiceMock = new Mock<ISessionService>();
    }

    #region GetAllActivitiesAsync
    [TestMethod]
    [TestCategory(nameof(ActivityService.GetAllActivitiesForUserAsync))]
    public async Task GetAllActivitiesAsync_Admin_AnothersActs_Ok()
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
        _assertActivitiesSame(gameDevAct, activitiesList[0]);
        _assertActivitiesSame(pianoAct, activitiesList[1]);
        _assertActivitiesSame(mcatAct, activitiesList[2]);

        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Never);
    }

    [TestMethod]
    [TestCategory(nameof(ActivityService.GetAllActivitiesForUserAsync))]
    public async Task GetAllActivitiesForUserAsync_NonAdmin_OwnActs_Ok()
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
        _assertActivitiesSame(gameDevAct, activitiesList[0]);
        _assertActivitiesSame(pianoAct, activitiesList[1]);
        _assertActivitiesSame(mcatAct, activitiesList[2]);

        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Never);
    }

    [TestMethod]
    [TestCategory(nameof(ActivityService.GetActivityAsync))]
    [TestCategory(nameof(ForbiddenException))]
    [ExpectedException(typeof(ForbiddenException))]
    public async Task GetAllActivitiesAsync_NonAdmin_AnothersActs_ThrowForbidden()
    {
        // -- Arrange --
        var activityService = _createActivityService();

        // -- Act --
        var activities = await activityService.GetAllActivitiesForUserAsync(JOHN_USER_GUID, JANE_USER_GUID);
    }

    [TestMethod]
    [TestCategory(nameof(ActivityService.GetActivityAsync))]
    public async Task GetAllActivitiesAsync_Admin_AnothersAct_DeletedAct_ReturnEmpty()
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
    [TestCategory(nameof(ActivityService.GetActivityAsync))]
    public async Task GetActivityAsync_Admin_AnothersAct_Ok(string activityIdStr)
    {
        // -- Arrange --
        var activityService = _createActivityService();

        // -- Act --
        Guid.TryParse(activityIdStr, out var activityGuid);
        var activity = await activityService.GetActivityAsync(JANE_USER_GUID, activityGuid);

        // -- Assert --
        Assert.IsNotNull(activity);
        _assertActivitiesSame(allActs.First(x => x.Id == activityGuid), activity);
    }

    [TestMethod]
    [DataRow(GAME_DEV_ACT_GUID_STR)]
    [DataRow(PIANO_ACT_GUID_STR)]
    [DataRow(MCAT_ACT_GUID_STR)]
    [TestCategory(nameof(ActivityService.GetActivityAsync))]
    public async Task GetActivityAsync_NonAdmin_OwnAct_Ok(string activityIdStr)
    {
        // -- Arrange --
        var activityService = _createActivityService();

        // -- Act --
        Guid.TryParse(activityIdStr, out var activityGuid);
        var activity = await activityService.GetActivityAsync(JOHN_USER_GUID, activityGuid);

        // -- Assert --
        Assert.IsNotNull(activity);
        _assertActivitiesSame(allActs.First(x => x.Id == activityGuid), activity);
        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Never);
    }

    [TestMethod]
    [TestCategory(nameof(ActivityService.GetActivityAsync))]
    [TestCategory(nameof(ForbiddenException))]
    [ExpectedException(typeof(ForbiddenException))]
    public async Task GetActivityAsync_NonAdmin_AnothersAct_ThrowForbidden()
    {
        // -- Arrange --
        var activityService = _createActivityService();

        // -- Act --
        var activity = await activityService.GetActivityAsync(JOHN_USER_GUID, PANIC_ACT_GUID);
    }

    [TestMethod]
    [TestCategory(nameof(ActivityService.GetActivityAsync))]
    public async Task GetActivityAsync_NonAdmin_OwnAct_NonExistentAct_ReturnNull()
    {
        // -- Arrange --
        var activityService = _createActivityService();

        // -- Act --
        var activity = await activityService.GetActivityAsync(JOHN_USER_GUID, NON_EXISTENT_GUID);

        // -- Assert --
        Assert.IsNull(activity);
        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Never);
    }

    [TestMethod]
    [TestCategory(nameof(ActivityService.GetActivityAsync))]
    public async Task GetActivityAsync_NonAdmin_OwnAct_DeletedAct_ReturnNull()
    {
        // -- Arrange --
        var activityService = _createActivityService();

        // -- Act --
        var activity = await activityService.GetActivityAsync(JOHN_USER_GUID, JOHNS_DELETED_ACT_GUID);

        // -- Assert --
        Assert.IsNull(activity);
        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Never);
    }
    #endregion GetActivityAsync

    #region CreateActivityAsync
    [TestMethod]
    [DataRow("NEW NAME", "NEW DESC", "2022-07-12T01:27:26Z", "2022-07-13T01:27:26Z", "2022-07-14T01:27:26Z", "#3366ff")]
    [TestCategory(nameof(ActivityService.CreateActivityAsync))]
    public async Task CreateActivityAsync_Admin_AnothersAct_Ok(
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
        _assertCreateOk(actCreateDto, activity);
    }

    [TestMethod]
    [TestCategory(nameof(ActivityService.CreateActivityAsync))]
    [TestCategory(nameof(ForbiddenException))]
    [ExpectedException(typeof(ForbiddenException))]
    public async Task CreateActivityAsync_NonAdmin_AnothersAct_ThrowForbidden()
    {
        // -- Arrange --
        var actCreateDto = new ActivityCreateDto();
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
    [TestCategory(nameof(ActivityService.CreateActivityAsync))]
    public async Task CreateActivityAsync_NonAdmin_OwnAct_Ok(
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
        _assertCreateOk(actCreateDto, activity);
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
    [TestCategory(nameof(ActivityService.CreateActivityAsync))]
    [TestCategory(nameof(InvalidDataException))]
    [ExpectedException(typeof(InvalidDataException))]
    public async Task CreateActivityAsync_NonAdmin_OwnAct_ThrowInvalidData(
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
    [TestCategory(nameof(ActivityService.UpdateActivityAsync))]
    public async Task UpdateActivityAsync_Admin_AnothersAct_Ok(
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
        _assertUpdateOk(GAME_DEV_ACT_GUID, actUpdateDto, activity);
    }

    [TestMethod]
    [DataRow("UPDATED NAME", "UPDATE DESC", MIN_DATE_STR, null, null, "#3366ff")]
    [TestCategory(nameof(ActivityService.UpdateActivityAsync))]
    public async Task UpdateActivityAsync_NonAdmin_OwnAct_StartDateMinDate_Ok(
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
        _assertUpdateOk(GAME_DEV_ACT_GUID, actUpdateDto, activity);
    }

    [TestMethod]
    [DataRow("UPDATED NAME", "UPDATE DESC", "2022-07-12T01:27:26Z", "2022-07-12T01:27:26Z", "2022-07-12T01:27:26Z", "#3366ff")]
    [DataRow("UPDATED NAME", "UPDATE DESC", "2022-07-12T01:27:26Z", "2022-07-13T01:27:26Z", "2022-07-14T01:27:26Z", "#3366ff")]
    [DataRow("UPDATED NAME", "UPDATE DESC", "2022-07-12T01:27:26Z", null, "2022-07-14T01:27:26Z", "#3366ff")]
    [DataRow("UPDATED NAME", "UPDATE DESC", "2022-07-12T01:27:26Z", "2022-07-13T01:27:26Z", null, "#3366ff")]
    [DataRow("UPDATED NAME", "UPDATE DESC", "2022-07-12T01:27:26Z", null, null, "#3366ff")]
    // StartDateUtc null so will be UtcNow.
    [DataRow("UPDATED NAME", "UPDATED DESC", null, null, null, "#3366ff")]
    [TestCategory(nameof(ActivityService.UpdateActivityAsync))]
    public async Task UpdateActivityAsync_NonAdmin_OwnAct_Ok(
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
        _assertUpdateOk(GAME_DEV_ACT_GUID, actUpdateDto, activity);
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
    [TestCategory(nameof(ActivityService.UpdateActivityAsync))]
    [TestCategory(nameof(InvalidDataException))]
    [ExpectedException(typeof(InvalidDataException))]
    public async Task UpdateActivityAsync_NonAdmin_OwnAct_ThrowInvalidData(
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
    }

    [TestMethod]
    [TestCategory(nameof(ActivityService.UpdateActivityAsync))]
    [TestCategory(nameof(ForbiddenException))]
    [ExpectedException(typeof(ForbiddenException))]
    public async Task UpdateActivityAsync_NonAdmin_AnothersAct_ThrowForbidden()
    {
        // -- Arrange --
        var actUpdateDto = new ActivityUpdateDto();
        var activityService = _createActivityService();

        // -- Act --
        var activity = await activityService.UpdateActivityAsync(JOHN_USER_GUID, PANIC_ACT_GUID, actUpdateDto);
    }
    // TODO add more tests
    #endregion UpdateActivityAsync

    #region DeleteActivityAsync
    [TestMethod]
    [TestCategory(nameof(ActivityService.DeleteActivityAsync))]
    public async Task DeleteActivityAsync_Admin_AnothersAct_Ok()
    {
        // -- Arrange --
        var activityService = _createActivityService();

        // -- Act --
        var isSuccessful = await activityService.DeleteActivityAsync(JANE_USER_GUID, GAME_DEV_ACT_GUID);

        // -- Assert --
        _assertDeleteOk(isSuccessful);
    }

    [TestMethod]
    [TestCategory(nameof(ActivityService.DeleteActivityAsync))]
    [TestCategory(nameof(ForbiddenException))]
    [ExpectedException(typeof(ForbiddenException))]

    public async Task DeleteActivityAsync_NonAdmin_AnothersAct_ThrowForbidden()
    {
        // -- Arrange --
        var activityService = _createActivityService();

        // -- Act --
        var isSuccessful = await activityService.DeleteActivityAsync(JOHN_USER_GUID, PANIC_ACT_GUID);
    }

    [TestMethod]
    [TestCategory(nameof(ActivityService.DeleteActivityAsync))]

    public async Task DeleteActivityAsync_NonAdmin_OwnAct_NonExistentAct_ReturnFalse()
    {
        // -- Arrange --
        var activityService = _createActivityService();

        // -- Act --
        var isSuccessful = await activityService.DeleteActivityAsync(JOHN_USER_GUID, NON_EXISTENT_GUID);

        // -- Assert --
        _assertDeleteReturnFalse(isSuccessful);
    }

    [TestMethod]
    [TestCategory(nameof(ActivityService.DeleteActivityAsync))]

    public async Task DeleteActivityAsync_NonAdmin_OwnAct_DeletedAct_ReturnFalse()
    {
        // -- Arrange --
        var activityService = _createActivityService();

        // -- Act --
        var isSuccessful = await activityService.DeleteActivityAsync(JOHN_USER_GUID, JOHNS_DELETED_ACT_GUID);

        // -- Assert --
        _assertDeleteReturnFalse(isSuccessful);
    }

    [TestMethod]
    [TestCategory(nameof(ActivityService.DeleteActivityAsync))]

    public async Task DeleteActivityAsync_NonAdmin_OwnAct_Ok()
    {
        // -- Arrange --
        var activityService = _createActivityService();

        // -- Act --
        var isSuccessful = await activityService.DeleteActivityAsync(JOHN_USER_GUID, GAME_DEV_ACT_GUID);

        // -- Assert --
        _assertDeleteOk(isSuccessful);
    }
    // TODO more tests
    #endregion DeleteActivityAsync

    private ActivityService _createActivityService()
    {
        return new ActivityService(
            dbContextMock.Object,
            userServiceMock.Object,
            _sessionsServiceMock.Object,
            mapperMock.Object);
    }

    private void _assertActivitiesSame(
        Activity expectedActivity,
        ActivityGetDto returnedActivity)
    {
        Assert.IsTrue(returnedActivity != null);
        Assert.AreEqual(expectedActivity.Id, returnedActivity.Id);
        Assert.AreEqual(expectedActivity.OwnerId, returnedActivity.OwnerId);
        Assert.AreEqual(expectedActivity.Name, returnedActivity.Name);
        Assert.AreEqual(expectedActivity.Description, returnedActivity.Description);
        Assert.AreEqual(expectedActivity.ColorHex, returnedActivity.ColorHex);
        Assert.AreEqual(expectedActivity.IsArchived, returnedActivity.IsArchived);
        _checkTags();
        _checkDates();

        // Check that the dates are equal within a minute
        void _checkDates()
        {
            Assert.IsTrue(DatesEqualWithinSeconds((DateTime)returnedActivity.StartDateUtc, DateTime.UtcNow));
            if (returnedActivity.DueDateUtc != null)
            {
                Assert.IsTrue(DatesEqualWithinSeconds((DateTime)returnedActivity.DueDateUtc, DateTime.UtcNow));
            }
            if (returnedActivity.CompletedDateUtc != null)
            {
                Assert.IsTrue(DatesEqualWithinSeconds((DateTime)returnedActivity.CompletedDateUtc, DateTime.UtcNow, 60));
            }
        }

        void _checkTags()
        {
            if (expectedActivity.Tags == null)
            {
                Assert.IsNull(returnedActivity.Tags);
            }
            else
            {
                Assert.IsNotNull(returnedActivity.Tags);
                Assert.AreEqual(expectedActivity.Tags.Count(), returnedActivity.Tags.Count());

                // Check tags in the expected activity contain all tags in the returned
                foreach (var tag in expectedActivity.Tags)
                {
                    returnedActivity.Tags.Contains(tag);
                }

                // Check tags in the returned activity contain all tags in the expected
                foreach (var tag in returnedActivity.Tags)
                {
                    expectedActivity.Tags.Contains(tag);
                }
            }
        }
    }

    private void _assertCreateOk(ActivityCreateDto createDto, ActivityGetDto returnedActivity)
    {
        // Verify returned object is as expected
        _assertReturnedFromCreate(createDto, returnedActivity);

        // Verify DB changed
        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Once);
    }

    private void _assertReturnedFromCreate(ActivityCreateDto createDto, ActivityGetDto returnedActivity)
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

    private void _assertUpdateOk(Guid activityId, ActivityUpdateDto updateDto, ActivityGetDto returnedActivity)
    {
        // Verify the returned object is as expected
        _assertReturnedFromUpdate(activityId, updateDto, returnedActivity);

        // Verify the DB is changed
        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Once);
    }

    private void _assertReturnedFromUpdate(Guid activityId, ActivityUpdateDto updateDto, ActivityGetDto returnedActivity)
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

    private void _assertDeleteOk(bool isSuccessful)
    {
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

    private void _assertDeleteReturnFalse(bool isSuccessful)
    {
        Assert.IsFalse(isSuccessful);
        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Never);
    }
}