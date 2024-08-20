using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasswordManager.Models;
using PasswordManager.Repository;
using PasswordManager.Responses;

namespace PasswordManager.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordController: ControllerBase {
        private readonly IPasswordRepository _passwordRepository;

        public PasswordController(IPasswordRepository passwordRepository) {
            _passwordRepository = passwordRepository;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostPasswordAsync(PasswordCreationDto data) 
        {
            ResponseData<Password> response = new ResponseData<Password>();
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

                response.Status = "success";
                response.Message = "Website password added successfully";
                response.Data = password;
                return Created("", response); // T.B.O

            }

            response.Status = "error";
            response.Message = "You are not authorized to access this resource";
            return Unauthorized(response);
        }

        // [Authorize]
        // [HttpGet("")]
        // public async Task<IActionResult> GetPasswords(int userId) {
        //     ResponseData<IEnumerable<Password>> response = new ResponseData<IEnumerable<Password>>();
        //     var passwords = await _passwordRepository.GetAllByValueAsync(userId);

        //     response.Status = "success";
        //     response.Message = "Request processed successfully";
        //     response.Data = passwords;
        //     return Ok(response);
        // }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> ViewPassword(int id) {
            ResponseData<Password> response = new ResponseData<Password>();

            var password = await _passwordRepository.GetByIdAsync(id);
            if (password == null) {
                response.Status = "error";
                response.Message = "Password not found"; // T.B.O
                return NotFound(response);
            }

            response.Status = "success";
            response.Message = "Password retrieved successfully"; 
            response.Data = password;
            return Ok(response);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePassword(int id, PasswordCreationDto data) {
            ResponseData<Password> response = new ResponseData<Password>();

            var existingPassword = await _passwordRepository.GetByIdAsync(id);

            if (existingPassword == null) {
                response.Status = "error";
                response.Message = "Password not found"; // T.B.O
                return BadRequest(response);
            }

            existingPassword.EmailOrUsername = data.EmailOrUsername;
            existingPassword.WebsiteUrl = data.WebsiteUrl;
            existingPassword.WebsitePassword = data.WebsitePassword;

            await _passwordRepository.UpdateAsync(existingPassword);

            response.Status = "success";
            response.Message = "Password updated successfully"; 
            response.Data = existingPassword;
            return Ok(response);

        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePassword(int id) {
            ResponseData<Password> response = new ResponseData<Password>();

            var existingPassword = await _passwordRepository.GetByIdAsync(id);

            if (existingPassword == null) {
                response.Status = "error";
                response.Message = "Password with Id " + id + " not found"; // T.B.O
                return BadRequest(response);
            }

            await _passwordRepository.DeleteAsync(existingPassword);

            response.Status = "success";
            response.Message = "Password deleted successfully"; 
            return Ok(response);
        }

        private int decodeJWT(string token) {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Add null check
            var userId = jwtToken.Claims.First(claim => claim.Type == "userId").Value;

            return int.Parse(userId);
            // add exception ?
        }


    }
}