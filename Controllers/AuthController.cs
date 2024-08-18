using Microsoft.AspNetCore.Mvc;
using PasswordManager.Models;
using System.Collections.Generic;
using AutoMapper;
using PasswordManager.DTOs;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace PasswordManager.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase {
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;

        public AuthController(IRepository<User> userRepository, IMapper mapper, IConfiguration configuration) {
            _userRepository = userRepository;
            _mapper = mapper;
            _secretKey = configuration["Jwt:SecretKey"];
            _issuer = configuration["Jwt:Issuer"];
            _audience = configuration["Jwt:Audience"];
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginDto data) {
            var user = await _userRepository.GetByValueAsync(u => u.Email == data.Email);

            if (user != null) {
                if (BCrypt.Net.BCrypt.Verify(data.Password, user.Password)) {
                    return Ok(new { Token = GenerateToken(data.Email)});
                }
                return Unauthorized();                    
            }
            return NotFound();

        }

        private string GenerateToken(string email) {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Email, email)
                }),
                Expires = DateTime.UtcNow.AddMinutes(30),
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