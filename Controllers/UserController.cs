using Microsoft.AspNetCore.Mvc;
using PasswordManager.Models;
using System.Collections.Generic;
using AutoMapper;
using PasswordManager.DTOs;
using BCrypt.Net;

namespace PasswordManager.Controllers
{
    public class UserController : ControllerBase {
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;

        public UserController(IRepository<User> userRepository, IMapper mapper) {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> PostUserAsync(UserCreationDto data) {
            data.Password = BCrypt.Net.BCrypt.HashPassword(data.Password);
            var mappedInput = _mapper.Map<User>(data);
            var user = await _userRepository.AddAsync(mappedInput);
            var result = _mapper.Map<UserCreationDto>(user);

            return Ok(result);
        }

        

    }
}