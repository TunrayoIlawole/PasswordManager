using Microsoft.AspNetCore.Mvc;
using PasswordManager.Models;
using System.Collections.Generic;
using AutoMapper;
using PasswordManager.DTOs;
using BCrypt.Net;
using PasswordManager.Repository;

namespace PasswordManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase {
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;

        public UserController(IRepository<User> userRepository, IMapper mapper) {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> PostUserAsync(UserCreationDto data) 
        {
            User newUser = new()
            {
                Email = data.Email,
                Username = data.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(data.Password)
            };
            // data.Password = BCrypt.Net.BCrypt.HashPassword(data.Password);
            // var mappedInput = _mapper.Map<User>(data);
            var user = await _userRepository.AddAsync(newUser);
            // var result = _mapper.Map<UserCreationDto>(user);

            return Ok(user);
        }

        

    }
}