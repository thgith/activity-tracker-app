using ActivityTrackerApp.Entities;
using ActivityTrackerApp.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ActivityTrackerApp.Database;

/// <summary>
/// Database context.
/// </summary>
public class DataContext : DbContext, IDataContext
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {

    }

    /// <summary>
    /// Users.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Activities.
    /// </summary>
    public DbSet<Activity> Activities { get; set; }

    /// <summary>
    /// Sessions.
    /// </summary>
    public DbSet<Session> Sessions { get; set; }

    /// <summary>
    /// Set up some configs for EF.
    /// </summary>
    /// <param name="modelBuilder">Used for configuration.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Convert the list of tags to a comma separated string to store in DB column
        var strListConverter = new StringListToStringValueConverter();
        var strValueComparer = new ValueComparer<IList<string>>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList());

        modelBuilder
            .Entity<Activity>()
            .Property(e => e.Tags)
            .HasConversion(strListConverter).Metadata.SetValueComparer(strValueComparer);
    }
}