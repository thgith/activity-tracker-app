using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ActivityTrackerApp.Entities;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;

namespace ActivityTrackerApp.Services
{
    /// <summary>
    /// JWT service.
    /// </summary>
    public class JwtService : IJwtService
    {
        private readonly IDataContext _dbContext;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public JwtService(
            IDataContext dataContext,
            IConfiguration config,
            IMapper mapper)
        {
            _dbContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public string GenerateJwtToken(User user)
        {
            // Get key in config
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtConfig:Secret"]));
            
            // Create credentials with key and selected signing algorithm
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Claims are to store data about user/process
            // Don't put any sensitive data here
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                // new Claim(ClaimTypes.Email, user.Email),
                // new Claim(ClaimTypes.GivenName, user.FirstName),
                // new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                _config["JwtConfig:Issuer"],
                _config["JwtConfig:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials);
            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public JwtSecurityToken Verify(string jwt)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["JwtConfig:Secret"]);
            tokenHandler.ValidateToken(jwt, new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false
            }, out SecurityToken validatedToken);

            return (JwtSecurityToken) validatedToken;
        }
    }
}