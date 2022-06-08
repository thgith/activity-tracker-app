using ActivityTrackerApp.Database;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Info("=== Starting ===");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Get the DB config from appsettings.json
    var dbConnectionString = builder.Configuration["ConnectionStrings:Postgres"];

    // Set up to use Postgres
    builder.Services.AddDbContext<DataContext>(options =>
    {
        options.UseNpgsql(dbConnectionString, b => b.MigrationsAssembly("ActivityTrackerApp"));
    });

    builder.Services.Configure<HostOptions>(hostOptions =>
    {
        hostOptions.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
    });

    var app = builder.Build();

    app.MapGet("/", () => "Hello World!");

    app.Run();
}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception" + exception.Message + exception.InnerException.ToString());
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}
