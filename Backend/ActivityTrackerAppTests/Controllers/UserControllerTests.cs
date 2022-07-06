// using ActivityTrackerApp.Controllers;
// using ActivityTrackerApp.Dtos;
// using ActivityTrackerApp.Services;
// using Microsoft.Extensions.Logging;
// using Moq;
// using NLog;

// namespace ActivityTrackerAppTests;

// [TestClass]
// public class UserControllerTests
// {
    
//     public UserControllerTests()
//     {

//     }

//     private void _setup()
//     {

//     }

//     // == UpdateUserAsync ==
//     [TestMethod]
//     public async void UpdateUserAsync_Member_Self()
//     {
//         // Arrange
//         var userServiceMock = new Mock<IUserService>();
//         var jwtServiceMock = new Mock<IJwtService>();
//         var helperServiceMock = new Mock<IHelperService>();
//         var loggerMock = new Mock<ILogger<UserController>>();

//         var authCtrl = new UserController(
//             userServiceMock.Object,
//             jwtServiceMock.Object,
//             helperServiceMock.Object,
//             loggerMock.Object);

//         Guid userId = Guid.NewGuid();
//         var userDto = new UserUpdateDto
//         {
//             FirstName = "Jane",
//             LastName = "Doe"
//         };

//         // Act
//         var res = await authCtrl.UpdateUserAsync(userId, userDto);

//         // Assert
//         Assert.AreEqual(res.ToString(), "");
//     }

//     public async void UpdateUserAsync_Member_Other()
//     {

//     }


//     public async void UpdateUserAsync_Unauthorized()
//     {

//     }
//     public async void UpdateUserAsync_ExpiredToken()
//     {

//     }

//     public async void UpdateUserAsync_Admin_Self()
//     {

//     }
//     public async void UpdateUserAsync_Admin_Other_UserDoesNotExist()
//     {

//     }
//     public async void UpdateUserAsync_Admin_Other_UserSoftDeleted()
//     {

//     }

//     // == GetAllAsync ==
//     public async void GetAllAsync_Unauthorized()
//     {

//     }
//     public async void GetAllAsync_ExpiredToken()
//     {

//     }

//     public async void GetAllAsync_Member()
//     {
        
//     }

//     public async void GetAllAsync_Admin()
//     {

//     }

//     public async void GetAllAsync_Err()
//     {

//     }

//     // == GetAsync ==
//     public async void GetAsync_Unauthorized()
//     {

//     }

//     public async void GetAsync_ExpiredToken()
//     {

//     }

//     public async void GetAsync_Member_Self()
//     {

//     }

//     public async void GetAsync_Member_Other()
//     {

//     }

//     public async void GetAsync_Admin_Self()
//     {

//     }

//     public async void GetAsync_Admin_Other_UserDoesNotExist()
//     {

//     }

//     public async void GetAsync_Admin_Other_UserSoftDeleted()
//     {

//     }

//     // == DeleteAsync ==
//     public async void DeleteUserAsync_Unauthorized()
//     {

//     }

//     public async void DeleteUserAsync_ExpiredToken()
//     {

//     }

//     public async void DeleteUserAsync_Member_Self()
//     {

//     }

//     public async void DeleteUserAsync_Member_Other()
//     {

//     }

//     public async void DeleteUserAsync_Admin_Self()
//     {

//     }

//     public async void DeleteUserAsync_Admin_Other()
//     {

//     }

//     public async void DeleteUserAsync_Admin_Other_UserSoftDeleted()
//     {

//     }

//     public async void DeleteUserAsync_Err()
//     {

//     }
// }