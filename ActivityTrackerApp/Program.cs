using ActivityTrackerApp.Database;
using ActivityTrackerApp.Services;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Info("=== Starting ===");

try
{
    var builder = WebApplication.CreateBuilder(args);
    var services = builder.Services;

    // Get the DB config from appsettings.json
    var dbConnectionString = builder.Configuration["ConnectionStrings:Postgres"];

    // Set up to use Postgres
    services.AddDbContext<DataContext>(options =>
    {
        options.UseNpgsql(dbConnectionString, b => b.MigrationsAssembly("ActivityTrackerApp"));
    });

    // Set up to map between entities and DTOs
    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

    // Inject data context into services
    services.AddScoped<IDataContext, DataContext>();
    services.AddScoped<IActivityService, ActivityService>();
    services.AddScoped<ISessionService, SessionService>();
    services.AddScoped<IUserService, UserService>();

    services.AddControllers();

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    // app.UseMvc();
    app.UseRouting();
    // app.UseAuthentication();
    // app.UseAuthorization();
    app.UseEndpoints(endpoints => endpoints.MapControllers());

    app.MapGet("/", () => "Hello World!");

    app.Run();

}
// This catch is to ignore the stop host error we get when running migrations.
catch (Exception ex) when (ex.GetType().Name == "StopTheHostException") { }
catch (Exception ex)
{
    // NLog: catch setup errors
    logger.Error(ex.StackTrace);
    logger.Error(ex, "Stopped program because of exception" + ex.Message + ex.InnerException.ToString());
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}