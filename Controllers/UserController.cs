using Microsoft.AspNetCore.Mvc;
using PasswordManager.Models;
using System.Collections.Generic;
using AutoMapper;
using PasswordManager.DTOs;
using PasswordManager.Repository;
using PasswordManager.Responses;
using Microsoft.AspNetCore.Authorization;

namespace PasswordManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordRepository _passwordRepository;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository, IPasswordRepository passwordRepository, IMapper mapper) {
            _userRepository = userRepository;
            _passwordRepository = passwordRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> PostUserAsync(UserCreationDto data) 
        {
            ResponseData<User> response = new ResponseData<User>();

            var existingUser = await _userRepository.GetByValueAsync(u => u.Email == data.Email);
            
            if (existingUser != null) {
                response.Status = "error";
                response.Message = "This user already exists"; // T.B.O
                return BadRequest(response);
            }

            User newUser = new()
            {
                Email = data.Email,
                Username = data.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(data.Password)
            };

            var user = await _userRepository.AddAsync(newUser);
            response.Status = "success";
            response.Message = "User created successfully";
            response.Data = user;

            return Ok(response);
        }

        [Authorize]
        [HttpGet("{userId}/passwords")]
        public async Task<IActionResult> GetPasswords(int userId) {

            // use id of logged in user?
            ResponseData<IEnumerable<Password>> response = new ResponseData<IEnumerable<Password>>();
            var passwords = await _passwordRepository.GetAllByValueAsync(userId);

            response.Status = "success";
            response.Message = "Request processed successfully";
            response.Data = passwords;
            return Ok(response);
        }

    }
}