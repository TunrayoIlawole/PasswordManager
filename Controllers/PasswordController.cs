using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasswordManager.Models;
using PasswordManager.Models.DTOs;
using PasswordManager.Responses;
using PasswordManager.Services;

namespace PasswordManager.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordController: ControllerBase {
        private readonly IPasswordService _passwordService;

        public PasswordController(IPasswordService passwordService) {
            _passwordService = passwordService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostPasswordAsync(PasswordCreationDto data) 
        {
            ResponseData<PasswordDto> response = new ResponseData<PasswordDto>();
            try {
                var authHeader = Request.Headers["Authorization"].ToString();

                if (authHeader != null && authHeader.StartsWith("Bearer ")) {
                    var token = authHeader.Substring("Bearer ".Length).Trim();
                    
                    PasswordDto password = await _passwordService.AddPassword(data, token);

                    response.Status = "success";
                    response.Message = "Website password added successfully";
                    response.Data = password;
                    return Created(password.Id.ToString(), response); 

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

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> ViewPassword(int id) {
            ResponseData<PasswordDetailDto> response = new ResponseData<PasswordDetailDto>();

            try {
                var authHeader = Request.Headers["Authorization"].ToString();
                if (authHeader != null && authHeader.StartsWith("Bearer ")) {
                    var token = authHeader.Substring("Bearer ".Length).Trim();

                    PasswordDetailDto password = await _passwordService.GetPassword(id, token);
                    response.Status = "success";
                    response.Message = "Password retrieved successfully"; 
                    response.Data = password;
                    return Ok(response);
                } else {
                    response.Status = "error";
                    response.Message = "You are not authorized to access this resource";
                    return Unauthorized(response);
                }

            } catch (InvalidEntityException e) {
                response.Status = "error";
                response.Message = e.Message;
                return NotFound(response);
            } catch (Exception e) {
                Console.WriteLine(e.StackTrace);
                response.Status = "error";
                response.Message = e.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePassword(int id, PasswordCreationDto data) {
            ResponseData<PasswordDto> response = new ResponseData<PasswordDto>();
            try {
                var authHeader = Request.Headers["Authorization"].ToString();
                if (authHeader != null && authHeader.StartsWith("Bearer ")) {
                    var token = authHeader.Substring("Bearer ".Length).Trim();

                    PasswordDto password = await _passwordService.UpdatePassword(id, data, token);

                    response.Status = "success";
                    response.Message = "Password updated successfully"; 
                    response.Data = password;
                    return Ok(response);
                } else {
                    response.Status = "error";
                    response.Message = "You are not authorized to access this resource";
                    return Unauthorized(response);
                }
                
            } catch (InvalidEntityException e) {
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePassword(int id) {
            ResponseData<Password> response = new ResponseData<Password>();

            try {
                var authHeader = Request.Headers["Authorization"].ToString();
                if (authHeader != null && authHeader.StartsWith("Bearer ")) {
                    var token = authHeader.Substring("Bearer ".Length).Trim();

                    await _passwordService.DeletePassword(id, token);

                    response.Status = "success";
                    response.Message = "Password deleted successfully"; 
                    return Ok(response);
                } else {
                    response.Status = "error";
                    response.Message = "You are not authorized to access this resource";
                    return Unauthorized(response);
                }
            } catch (InvalidEntityException e) {
                response.Status = "error";
                response.Message = e.Message;
                return BadRequest(response);
            } catch (Exception e) {
                response.Status = "error";
                response.Message = e.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

    }
}