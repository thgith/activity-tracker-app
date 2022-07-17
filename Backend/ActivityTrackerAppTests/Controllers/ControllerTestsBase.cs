using Microsoft.EntityFrameworkCore;

using ActivityTrackerApp.Database;
using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Entities;
using ActivityTrackerApp.Services;

using AutoMapper;

using MockQueryable.Moq;

using Moq;

using static ActivityTrackerAppTests.Fixtures.TestFixtures;
using Microsoft.Extensions.Logging;

namespace ActivityTrackerAppTests;

[TestClass]
public abstract class ControllerTestsBase<T>
{
    protected static Mock<IUserService> userServiceMock;
    protected static Mock<ISessionService> sessionServiceMock;
    protected static Mock<IJwtService> jwtServiceMock;
    protected static Mock<ILogger<T>> loggerMock;
    protected static Mock<IMapper> mapperMock;

    protected static void initializeClass()
    {
    }
}