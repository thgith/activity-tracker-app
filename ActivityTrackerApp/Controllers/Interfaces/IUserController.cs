using ActivityTrackerApp.Dtos;
using ActivityTrackerApp.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ActivityTrackerApp.Controllers
{
    /// <summary>
    /// User endpoints.
    /// </summary>
    public interface IUserController
    {
        [HttpGet]
        Task<ActionResult<IEnumerable<UserDto>>> GetAllAsync();

        [HttpGet]
        Task<ActionResult<UserDto>> GetAsync(Guid userId);
    }
}