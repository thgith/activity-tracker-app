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
// TODO test 
//  startDateUtc null
//  startDateUtc > dueDateUtc
//  startDateUtc > completedDateUtc
//  startDateUtc is MIN_DATE
//  etc other paths
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
    [TestCategory("GetActivityAsync")]
    public async Task GetActivityAsync_Admin_AnothersActivity_Ok()
    {
        // -- Arrange --
        var activityService = _createActivityService();

        // -- Act --
        var activity = await activityService.GetActivityAsync(JANE_USER_GUID, GAME_DEV_ACT_GUID);

        // -- Assert --
        Assert.IsNotNull(activity);
        _assertActivitiesSame(activity, gameDevAct);
    }

    [TestMethod]
    [TestCategory("GetActivityAsync")]
    public async Task GetActivityAsync_NonAdmin_OwnActivity_Ok()
    {
        // -- Arrange --
        var activityService = _createActivityService();

        // -- Act --
        var activity = await activityService.GetActivityAsync(JOHN_USER_GUID, GAME_DEV_ACT_GUID);

        // -- Assert --
        Assert.IsNotNull(activity);
        _assertActivitiesSame(activity, gameDevAct);
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
    }

    #endregion GetActivityAsync

    #region CreateActivityAsync

    [TestMethod]
    [TestCategory("CreateActivityAsync")]
    public async Task CreateActivityAsync_Admin_AnothersActivity_Ok()
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

        var actEntity = new Activity
        {
            Id = Guid.NewGuid(),
            OwnerId = JOHN_USER_GUID,
            Name = actCreateDto.Name,
            Description = actCreateDto.Description,
            StartDateUtc = actCreateDto.StartDateUtc,
            DueDateUtc = actCreateDto.DueDateUtc,
            CompletedDateUtc = actCreateDto.CompletedDateUtc,
            IsArchived = false,
            ColorHex = actCreateDto.ColorHex,
            Tags = actCreateDto.Tags,
        };

        mapperMock.Setup(x => x.Map<Activity>(actCreateDto))
                    .Returns(actEntity);

        var activityService = _createActivityService();

        // -- Act --
        var activity = await activityService.CreateActivityAsync(JANE_USER_GUID, JOHN_USER_GUID, actCreateDto);

        // -- Assert --
        // Verify returned object is as expected
        Assert.IsNotNull(activity);
        Assert.AreEqual(actCreateDto.Name, activity.Name);
        Assert.AreEqual(actCreateDto.Description, activity.Description);
        // TODO: NEED TO WRAP DateTime to be able to mock it
        // Assert.IsTrue(DatesEqualWithinSeconds((DateTime) activity.StartDateUtc, DateTime.UtcNow));
        Assert.IsNull(activity.DueDateUtc);
        Assert.IsNull(activity.CompletedDateUtc);
        Assert.AreEqual(actCreateDto.ColorHex, activity.ColorHex);
        Assert.IsFalse(activity.IsArchived);
        Assert.IsNull(activity.Tags);

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

    // TODO more tests
    #endregion CreateActivityAsync

    #region UpdateActivityAsync

    [TestMethod]
    [TestCategory("UpdateActivityAsync")]

    public async Task UpdateActivityAsync_Admin_AnothersActivity_Ok()
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

        var actEntity = new Activity
        {
            Id = Guid.NewGuid(),
            OwnerId = JOHN_USER_GUID,
            Name = actUpdateDto.Name,
            Description = actUpdateDto.Description,
            StartDateUtc = actUpdateDto.StartDateUtc,
            DueDateUtc = actUpdateDto.DueDateUtc,
            CompletedDateUtc = actUpdateDto.CompletedDateUtc,
            IsArchived = false,
            ColorHex = actUpdateDto.ColorHex,
            Tags = actUpdateDto.Tags,
        };

        mapperMock.Setup(x => x.Map<Activity>(actUpdateDto))
                    .Returns(actEntity);

        var activityService = _createActivityService();

        // -- Act --
        var activity = await activityService.UpdateActivityAsync(JANE_USER_GUID, GAME_DEV_ACT_GUID, actUpdateDto);

        // -- Assert --
        // Verify the returned object is as expected
        Assert.IsNotNull(activity);
        Assert.AreEqual(actUpdateDto.Name, activity.Name);
        Assert.AreEqual(actUpdateDto.Description, activity.Description);
        // TODO: NEED TO WRAP DateTime to be able to mock it
        // Assert.IsTrue(DatesEqualWithinSeconds((DateTime) activity.StartDateUtc, DateTime.UtcNow));
        DatesEqualWithinSeconds((DateTime)actUpdateDto.DueDateUtc, (DateTime)activity.DueDateUtc);
        DatesEqualWithinSeconds((DateTime)actUpdateDto.CompletedDateUtc, (DateTime)activity.CompletedDateUtc);
        Assert.AreEqual(actUpdateDto.ColorHex, activity.ColorHex);
        Assert.IsFalse(activity.IsArchived);
        Assert.AreEqual(2, activity.Tags.Count());

        // Verify the DB is changed
        dbContextMock.Verify(m => m.SaveChangesAsync(default(CancellationToken)), Times.Once);
    }

    [TestMethod]
    [TestCategory("UpdateActivityAsync")]
    public async Task UpdateActivityAsync_NonAdmin_OwnActivity_Ok()
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

        var actEntity = new Activity
        {
            Id = Guid.NewGuid(),
            OwnerId = JOHN_USER_GUID,
            Name = actUpdateDto.Name,
            Description = actUpdateDto.Description,
            StartDateUtc = actUpdateDto.StartDateUtc,
            DueDateUtc = actUpdateDto.DueDateUtc,
            CompletedDateUtc = actUpdateDto.CompletedDateUtc,
            IsArchived = false,
            ColorHex = actUpdateDto.ColorHex,
            Tags = actUpdateDto.Tags,
        };

        mapperMock.Setup(x => x.Map<Activity>(actUpdateDto))
                    .Returns(actEntity);

        var activityService = _createActivityService();

        // -- Act --
        var activity = await activityService.UpdateActivityAsync(JOHN_USER_GUID, GAME_DEV_ACT_GUID, actUpdateDto);

        // -- Assert --
        // Verify the returned object is as expected
        Assert.IsNotNull(activity);
        Assert.AreEqual(actUpdateDto.Name, activity.Name);
        Assert.AreEqual(actUpdateDto.Description, activity.Description);
        // TODO: NEED TO WRAP DateTime to be able to mock it
        // Assert.IsTrue(DatesEqualWithinSeconds((DateTime) activity.StartDateUtc, DateTime.UtcNow));
        DatesEqualWithinSeconds((DateTime)actUpdateDto.DueDateUtc, (DateTime)activity.DueDateUtc);
        DatesEqualWithinSeconds((DateTime)actUpdateDto.CompletedDateUtc, (DateTime)activity.CompletedDateUtc);
        Assert.AreEqual(actUpdateDto.ColorHex, activity.ColorHex);
        Assert.IsFalse(activity.IsArchived);
        Assert.AreEqual(2, activity.Tags.Count());

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
        var activityService =_createActivityService();

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
        ActivityGetDto activityGetDto,
        Activity activity)
    {
        Assert.IsTrue(activityGetDto != null);
        Assert.AreEqual(activity.Id, activityGetDto.Id);
        Assert.AreEqual(activity.OwnerId, activityGetDto.OwnerId);
        Assert.AreEqual(activity.Name, activityGetDto.Name);
        Assert.AreEqual(activity.Description, activityGetDto.Description);
        Assert.AreEqual(activity.ColorHex, activityGetDto.ColorHex);
        Assert.AreEqual(activity.IsArchived, activityGetDto.IsArchived);

        if (activity.Tags == null)
        {
            Assert.IsNull(activityGetDto.Tags);
        }
        else
        {
            Assert.IsNotNull(activityGetDto.Tags);
            Assert.AreEqual(activity.Tags.Count(), activityGetDto.Tags.Count());
            foreach (var tag in activity.Tags)
            {
                activityGetDto.Tags.Contains(tag);
            }
            foreach (var tag in activityGetDto.Tags)
            {
                activity.Tags.Contains(tag);
            }
        }

        // Check that the dates are equal within a minute
        Assert.IsTrue(DatesEqualWithinSeconds((DateTime)activityGetDto.StartDateUtc, DateTime.UtcNow));
        if (activityGetDto.DueDateUtc != null)
        {
            Assert.IsTrue(DatesEqualWithinSeconds((DateTime)activityGetDto.DueDateUtc, DateTime.UtcNow));
        }
        if (activityGetDto.CompletedDateUtc != null)
        {
            Assert.IsTrue(DatesEqualWithinSeconds((DateTime)activityGetDto.CompletedDateUtc, DateTime.UtcNow, 60));
        }
    }
}