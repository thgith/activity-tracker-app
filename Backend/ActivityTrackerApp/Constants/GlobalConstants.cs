namespace ActivityTrackerApp.Constants;

/// <summary>
/// Constants.
/// </summary>
public static class GlobalConstants
{
    /// <summary>
    /// General name of the application.
    /// </summary>
    public const string APP_NAME = "Activity Tracker";

    /// <summary>
    /// Name of the cookie to store the JWT token.
    /// </summary>
    public const string JWT_TOKEN_COOKIE_NAME = "jwt";

    /// <summary>
    /// The number of hours the auth cookie is valid.
    /// </summary>
    /// <remarks>This matches the amount of time the JWT token is valid.</remarks>
    public const int AUTH_COOKIE_EXPIRATION_HOURS = 5;

    /// <summary>
    /// Name of the cookie storing only the current user ID.
    /// </summary>
    public const string CURR_USER_ID_COOKIE_NAME = "curr_user_id";

    /// <summary>
    /// Location of the JWT secret in the appsettings.
    /// </summary>
    public const string JWT_SECRET_KEY_NAME = "JwtConfig:Secret";

    /// <summary>
    /// Location of the JWT issuer in the appsettings.
    /// </summary>
    public const string JWT_ISSUER_KEY_NAME = "JwtConfig:Issuer";

    /// <summary>
    /// Location of the JWT audience in the appsettings.
    /// </summary>
    public const string JWT_AUDIENCE_KEY_NAME = "JwtConfig:Audience";

    /// <summary>
    /// The max character length for short text.
    /// </summary>
    public const int SHORT_TEXT_MAX_CHAR = 50;
    
    /// <summary>
    /// The max character length for long text.
    /// </summary>
    public const int LONG_TEXT_MAX_CHAR = 1024;
}