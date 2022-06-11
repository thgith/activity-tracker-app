using System.IdentityModel.Tokens.Jwt;
using ActivityTrackerApp.Entities;

namespace ActivityTrackerApp.Services
{
    public interface IJwtService
    {
        public JwtSecurityToken CheckAuthenticated(string jwtCookie);
        public string GenerateJwtToken(User user, int expirationMinutes = 300);
        public JwtSecurityToken Verify(string jwt);
    }
}