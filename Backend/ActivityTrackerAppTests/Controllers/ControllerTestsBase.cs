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
// TODO: write these tests.
// Go to services to see tests that are actually written
// Here will be similar but also need to set up the Request object.
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