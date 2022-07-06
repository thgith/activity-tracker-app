namespace ActivityTrackerApp.Constants;
public static class GlobalConstants
{
    public const string APP_NAME = "Activity Tracker";
    public const string JWT_TOKEN_COOKIE_NAME = "jwt";
    // NOTE: This matches the amount of time the JWT token is valid
    public const int AUTH_COOKIE_EXPIRATION_HOURS = 5;
    public const string CURR_USER_ID_COOKIE_NAME = "curr_user_id";
    public const string JWT_SECRET_KEY_NAME = "JwtConfig:Secret";
    public const string JWT_ISSUER_KEY_NAME = "JwtConfig:Issuer";
    public const string JWT_AUDIENCE_KEY_NAME = "JwtConfig:Audience";
}