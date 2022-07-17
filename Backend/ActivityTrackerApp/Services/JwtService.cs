using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.Tokens;

using ActivityTrackerApp.Constants;
using ActivityTrackerApp.Database;
using ActivityTrackerApp.Entities;
using ActivityTrackerApp.Exceptions;

using AutoMapper;

namespace ActivityTrackerApp.Services
{
    /// <inheritdoc/>
    public class JwtService : IJwtService
    {
        private readonly IDataContext _dbContext;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor.
        /// </summary>
        public JwtService(
            IDataContext dataContext,
            IConfiguration config,
            IMapper mapper)
        {
            _dbContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <inheritdoc/>
        public JwtSecurityToken CheckAuthenticated(string jwtCookie)
        {
            if (jwtCookie == null)
            {
                throw new UnauthenticatedException("You are not properly authenticated");
            }

            // Verify that the token is still valid
            JwtSecurityToken token;
            try
            {
                token = _verify(jwtCookie);
            }
            catch (Exception _e)
            {
                throw new UnauthenticatedException("You are not properly authenticated");
            }

            return token;
        }

        /// <inheritdoc/>
        public string GenerateJwtToken(User user, int expirationMinutes = 300)
        {
            // Get key in config
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config[GlobalConstants.JWT_SECRET_KEY_NAME]));

            // Create credentials with key and selected signing algorithm
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Claims are to store data about user/process
            // Don't put any sensitive data here
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            var token = new JwtSecurityToken(
                _config[GlobalConstants.JWT_ISSUER_KEY_NAME],
                _config[GlobalConstants.JWT_AUDIENCE_KEY_NAME],
                claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private JwtSecurityToken _verify(string jwt)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config[GlobalConstants.JWT_SECRET_KEY_NAME]);
            tokenHandler.ValidateToken(jwt, new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _config[GlobalConstants.JWT_ISSUER_KEY_NAME],
                ValidAudience = _config[GlobalConstants.JWT_AUDIENCE_KEY_NAME]
            }, out SecurityToken validatedToken);

            return (JwtSecurityToken)validatedToken;
        }
    }
}