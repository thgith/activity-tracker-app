using ActivityTrackerApp.Entities;
using Microsoft.EntityFrameworkCore;

public interface IDataContext
{
    DbSet<User> Users { get; set; }
    DbSet<Activity> Activities { get; set; }
    DbSet<Session> Sessions { get; set; }
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}