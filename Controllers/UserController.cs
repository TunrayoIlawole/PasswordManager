using Microsoft.AspNetCore.Mvc;
using PasswordManager.Models;
using System.Collections.Generic;
using AutoMapper;
using PasswordManager.DTOs;
using PasswordManager.Repository;
using PasswordManager.Responses;
using Microsoft.AspNetCore.Authorization;
using PasswordManager.Services;

namespace PasswordManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase {

        private readonly IUserService _userService;
        private readonly IPasswordService _passwordService;

        public UserController(IUserService userService) {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> PostUserAsync(UserCreationDto data) 
        {
            ResponseData<UserCreatedDto> response = new ResponseData<UserCreatedDto>();
            try {
                UserCreatedDto user = await _userService.AddUser(data);
                response.Status = "success";
                response.Message = "User created successfully";
                response.Data = user;

                return Created("", response);
            } catch (DuplicateEntityException e) {
                response.Status = "error";
                response.Message = e.Message;
                return BadRequest(response);
            } catch (Exception e) {
                response.Status = "error";
                response.Message = e.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [Authorize]
        [HttpGet("{userId}/passwords")]
        public async Task<IActionResult> GetPasswords(int userId) {

            ResponseData<List<PasswordDto>> response = new ResponseData<List<PasswordDto>>();

            try {
                var authHeader = Request.Headers["Authorization"].ToString();

                if (authHeader != null && authHeader.StartsWith("Bearer ")) {
                    var token = authHeader.Substring("Bearer ".Length).Trim();
                    
                    List<PasswordDto> passwords = await _passwordService.GetPasswords(token);

                    response.Status = "success";
                    response.Message = "Website password added successfully";
                    response.Data = passwords;
                    return Ok(response);

                } else {
                    response.Status = "error";
                    response.Message = "You are not authorized to access this resource";
                    return Unauthorized(response);
                }
            } catch (InvalidOperationException e) {
                    response.Status = "error";
                    response.Message = e.Message;
                    return Unauthorized(response);
            } catch (Exception e) {
                response.Status = "error";
                response.Message = e.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

    }
}