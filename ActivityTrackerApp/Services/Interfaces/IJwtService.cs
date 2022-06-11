using System.IdentityModel.Tokens.Jwt;
using ActivityTrackerApp.Entities;

namespace ActivityTrackerApp.Services
{
    public interface IJwtService
    {
        public string GenerateJwtToken(User user);
        public JwtSecurityToken Verify(string jwt);
    }
}