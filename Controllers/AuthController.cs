using Microsoft.AspNetCore.Mvc;
using PasswordManager.Models;
using System.Collections.Generic;
using AutoMapper;
using PasswordManager.DTOs;
using BCrypt.Net;

namespace PasswordManager.Controllers
{
    public class AuthController : ControllerBase {
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;

        public AuthController(IRepository<User> userRepository, IMapper mapper) {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginDto data) {
            var user = await _userRepository.GetByValueAsync(u => u.Email == data.Email);

            if (user != null) {
                if (BCrypt.Net.BCrypt.Verify(data.Password, user.Password)) {
                    return Ok(new { Token = });
                }

// To be updated?
                return BadRequest();
            }
            return NotFound();

        }

        

    }
}