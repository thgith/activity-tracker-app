using ActivityTrackerApp.Models;
using ActivityTrackerApp.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

public interface IDataContext
{
    DbSet<User> Users { get; set; }
    DbSet<Activity> Activities { get; set; }
    DbSet<Session> Sessions { get; set; }
}