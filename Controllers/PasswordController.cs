using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Mvc;
using PasswordManager.Models;
using PasswordManager.Repository;

namespace PasswordManager.Controllers {
    public class PasswordController: ControllerBase {
        private readonly IRepository<Password> _passwordRepository;

        public PasswordController(IRepository<Password> passwordRepository) {
            _passwordRepository = passwordRepository;
        }

        [HttpPost]
        public async Task<IActionResult> PostPasswordAsync(PasswordCreationDto data) 
        {
            var authHeader =Request.Headers["Authorization"].ToString();

            if (authHeader != null && authHeader.StartsWith("Bearer ")) {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                Password newPassword = new()
                {
                    EmailOrUsername = data.EmailOrUsername,
                    WebsiteUrl = data.WebsiteUrl,
                    WebsitePassword = BCrypt.Net.BCrypt.HashPassword(data.WebsitePassword),
                    UserId = decodeJWT(token)
                };

                var password = await _passwordRepository.AddAsync(newPassword);

                // To be updated
                return Created("", password);

            }

            return Unauthorized();
        }

        [HttpGet]
        public async Task<IActionResult> GetPasswords(int userId) {
            var passwords = await _passwordRepository.GetAllByValueAsync(userId);

            return Ok(passwords);
        }

        [HttpGet("/{id}")]
        public async Task<IActionResult> ViewPassword(int id) {
            var password = await _passwordRepository.GetByIdAsync(id);
            if (password == null) {
                return NotFound();
            }
            // decrypt password?
            return Ok(password);
        }

        private int decodeJWT(string token) {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var userId = jwtToken.Claims.First(claim => claim.Type == "id").Value;

            return int.Parse(userId);
            // add exception ?
        }


    }
}