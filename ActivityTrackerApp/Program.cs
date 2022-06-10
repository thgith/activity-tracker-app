using System.Text;
using ActivityTrackerApp.Database;
using ActivityTrackerApp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(jwt => {
        // Used to encrypt our JWT tokens
        var key = Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Secret"]);

        // Set up encryption settings
        jwt.SaveToken = true;
        jwt.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            // Enable this later. Then set in appsettings.json
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            // Change this to true when not testing
            RequireExpirationTime = false
        };
    });
    
    services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<DataContext>();

    // Set up to map between entities and DTOs
    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

    // Inject data context into services
    services.AddScoped<IDataContext, DataContext>();
    services.AddScoped<IActivityService, ActivityService>();
    services.AddScoped<ISessionService, SessionService>();
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<IHelperMethods, HelperMethods>();

    services.AddControllers();

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();

    // Allow API to authenticate users
    app.UseAuthentication();
    app.UseAuthorization();

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