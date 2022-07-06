using System.IdentityModel.Tokens.Jwt;
using ActivityTrackerApp.Entities;

namespace ActivityTrackerApp.Services
{
    /// <summary>
    /// Handles JWT token authentication.
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Checks whether the cookie has a valid JWT token for auth.
        /// </summary>
        /// <param name="jwtCookie">The JWT cookie string.</param>
        public JwtSecurityToken CheckAuthenticated(string jwtCookie);

        /// <summary>
        /// Generates the JWT token for authentication.
        /// </summary>
        /// <param name="user">The user to generate the token for.</param>
        /// <param name="expirationMinutes">
        /// The amount of time the token is valid in minutes.
        /// </param>
        /// <remarks>
        /// Default expiration to 5 hours
        /// Prob not the best, but this is a time tracking application, so we don't want to
        /// log them out mid session
        /// </remarks>
        public string GenerateJwtToken(User user, int expirationMinutes = 300);
    }
}