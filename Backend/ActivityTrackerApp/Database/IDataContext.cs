using ActivityTrackerApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace ActivityTrackerApp.Database;

/// <summary>
/// Database context.
/// </summary>
public interface IDataContext
{
    /// <summary>
    /// Users
    /// </summary>
    DbSet<User> Users { get; set; }

    /// <summary>
    /// Activities
    /// </summary>
    DbSet<Activity> Activities { get; set; }

    /// <summary>
    /// Sessions
    /// </summary>
    DbSet<Session> Sessions { get; set; }

    /// <summary>
    /// Save changes to DB.
    /// </summary>
    int SaveChanges();
    
    /// <summary>
    /// Save changes to DB asynchronously.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}