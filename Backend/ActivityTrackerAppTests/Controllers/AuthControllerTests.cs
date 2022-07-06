// using ActivityTrackerApp.Controllers;
// using ActivityTrackerApp.Dtos;
// using ActivityTrackerApp.Services;
// using AutoMapper;
// using Microsoft.Extensions.Logging;
// using Moq;
// using NLog;

// namespace ActivityTrackerAppTests;

// [TestClass]
// public class AuthControllerTests
// {
    
//     public AuthControllerTests()
//     {

//     }

//     private void _setup()
//     {

//     }


//     [TestMethod]
//     public async void RegisterAsync()
//     {
//         // Arrange
//         var userServiceMock = new Mock<IUserService>();
//         var helperServiceMock = new Mock<IHelperService>();
//         var mapperMock = new Mock<IMapper>();
//         var loggerMock = new Mock<ILogger<AuthController>>();

//         var authCtrl = new AuthController(
//             userServiceMock.Object,
//             helperServiceMock.Object,
//             mapperMock.Object,
//             loggerMock.Object);

//         var userRegisterDto = new UserRegisterDto
//         {
//             FirstName = "Jane",
//             LastName = "Doe",
//             Email = "janedoe@test.com",
//             Password = "12345678"
//         };

//         var entityWithToken = new EntityWithToken<UserRegisterDto>
//         {
//             Entity = null,
//             Token = ""
//         };

//         userServiceMock
//             .Setup(x => x.RegisterUserAsync(userRegisterDto))
//             .Returns(Task.FromResult(entityWithToken));

//         // Act
//         var response = await authCtrl.RegisterAsync(userRegisterDto);  
        
//         // Assert
//         Assert.AreEqual(response.ToString(), "");
//     }

//     [TestMethod]
//     public async void RegisterAsync_NoFirstName()
//     {

//     }

//     [TestMethod]
//     public async void RegisterAsync_NoLastName()
//     {

//     }

//     [TestMethod]
//     public async void RegisterAsync_NoEmail()
//     {

//     }

//     [TestMethod]
//     public async void RegisterAsync_NoPassword()
//     {

//     }
    
//     [TestMethod]
//     public async void RegisterAsync_UserWithEmailAlreadyExists()
//     {

//     }

//     [TestMethod]
//     public async void RegisterAsync_Err()
//     {

//     }

//     // == LoginAsync ==
//     [TestMethod]
//     public void LoginAsync_CredentialsValid()
//     {
//         // Arrange

//         // Act
        
//         // Assert
//     }

//     [TestMethod]
//     public void LoginAsync_UserDoesNotExist()
//     {
//         // Arrange

//         // Act
        
//         // Assert
//     }

//     [TestMethod]
//     public void LoginAsync_WrongPassword()
//     {
//         // Arrange

//         // Act
        
//         // Assert
//     }

//     [TestMethod]
//     public void Logout()
//     {
//         // Arrange

//         // Act
        
//         // Assert
//     }
// }