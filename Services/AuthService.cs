using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PasswordManager.DTOs;
using PasswordManager.Models;
using PasswordManager.Repository;

namespace PasswordManager.Services {

    public class AuthService: IAuthService {

        public readonly IUserRepository _userRepository;
         private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;

        public AuthService(IUserRepository userRepository, IConfiguration configuration) {
            _userRepository = userRepository;
            _secretKey = configuration["Jwt:SecretKey"];
            _issuer = configuration["Jwt:Issuer"];
            _audience = configuration["Jwt:Audience"];
        }

        public async Task<string> SignInUser(LoginDto loginDto) {
            User user = await _userRepository.GetByValueAsync(u => u.Email == loginDto.Email);

            if (user != null) {
                if (BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password)) {
                    return GenerateToken(user.Email, user.Id);
                } else {
                    throw new UnauthorizedAccessException();
                }                  
            }
            throw new InvalidEntityException("Email " + loginDto.Email + "not found");
        }

        private string GenerateToken(string email, int userId) {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity([
                    new Claim("userId", userId.ToString()),
                    new Claim(ClaimTypes.Email, email)
                ]),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }
    }
    
}