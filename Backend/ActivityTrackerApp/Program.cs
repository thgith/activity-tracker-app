using System.Reflection;
using System.Security.Claims;
using System.Text;
using ActivityTrackerApp.Constants;
using ActivityTrackerApp.Database;
using ActivityTrackerApp.Services;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);
    var services = builder.Services;

    // Set up CORS configuration
    services.AddCors(options =>
    {
        options.AddPolicy(name: "CorsPolicy",
                          policy =>
                          {
                              policy.WithOrigins("http://localhost:3000",
                                                "https://localhost:3000",
                                                "http://localhost:3000/",
                                                "https://localhost:3000/")

                                                        .AllowAnyHeader()
        .AllowAnyMethod()
        // This is so we can store the JWT token in cookies
        .AllowCredentials()
                                                ;
                          });
    });

    services.AddAuthorization(options =>
    {
        options.AddPolicy("UsersOnly", policy => policy.RequireClaim(ClaimTypes.Role, Roles.MEMBER, Roles.ADMIN));
    });

    // Get the DB config from appsettings.json
    var dbConnectionString = builder.Configuration["ConnectionStrings:Postgres"];

    // Set up to use Postgres
    services.AddDbContext<DataContext>(options =>
    {
        options.UseNpgsql(dbConnectionString, b => b.MigrationsAssembly("ActivityTrackerApp"));
    });

    services.AddAuthentication(
        CertificateAuthenticationDefaults.AuthenticationScheme)
        .AddCertificate();

    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(jwt =>
    {
        // Used to encrypt our JWT tokens
        var key = Encoding.UTF8.GetBytes(builder.Configuration[GlobalConstants.JWT_SECRET_KEY_NAME]);

        // Set up encryption settings
        jwt.SaveToken = true;
        jwt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration[GlobalConstants.JWT_ISSUER_KEY_NAME],
            ValidAudience = builder.Configuration[GlobalConstants.JWT_AUDIENCE_KEY_NAME],
            RequireExpirationTime = true
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
    services.AddScoped<IJwtService, JwtService>();
    services.AddScoped<IHelperService, HelperService>();

    services.AddControllers();

    services.AddSwaggerGen(x =>
    {
        // Show XML docs in Swagger
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        x.IncludeXmlComments(xmlPath);
    });

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();

    // Allow CORS for frontend
    app.UseCors("CorsPolicy");

    // Allow API to authenticate users
    app.UseAuthentication();
    app.UseAuthorization();

    app.UseEndpoints(endpoints => endpoints.MapControllers());

    app.UseSwagger();
    // Swagger endpoint now available at /swagger
    app.UseSwaggerUI(x => x.SwaggerEndpoint("/swagger/v1/swagger.json", "Activity Tracker API v1"));

    app.MapGet("/", () => "Activity Tracker API ready for requests!");

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