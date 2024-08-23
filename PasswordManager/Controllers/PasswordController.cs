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

                    response.Status = ResponseMessages.Success;;
                    response.Message = ResponseMessages.SucessfullAction("Website password", "added"); 
                    response.Data = password;
                    return Created(password.Id.ToString(), response); 

                } else {
                    response.Status = ResponseMessages.Failure;;
                    response.Message = ResponseMessages.Unauthorized;
                    return Unauthorized(response);
                }
            } catch (InvalidOperationException e) {
                response.Status = ResponseMessages.Failure;
                response.Message = e.Message;
                return Unauthorized(response);
            } catch (Exception e) {
                response.Status = ResponseMessages.Failure;
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
                    response.Status = ResponseMessages.Success;
                    response.Message = ResponseMessages.SucessfullAction("Password", "retrieved"); 
                    response.Data = password;
                    return Ok(response);
                } else {
                    response.Status = ResponseMessages.Failure;
                    response.Message = ResponseMessages.Unauthorized;
                    return Unauthorized(response);
                }

            } catch (InvalidEntityException e) {
                response.Status = ResponseMessages.Failure;
                response.Message = e.Message;
                return NotFound(response);
            } catch (Exception e) {
                // log exception
                Console.WriteLine(e.StackTrace);
                response.Status = ResponseMessages.Failure;
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

                    response.Status = ResponseMessages.Success;
                    response.Message = ResponseMessages.SucessfullAction("Password", "updated"); 
                    response.Data = password;
                    return Ok(response);
                } else {
                    response.Status = ResponseMessages.Failure;
                    response.Message = ResponseMessages.Unauthorized;
                    return Unauthorized(response);
                }
                
            } catch (InvalidEntityException e) {
                response.Status = ResponseMessages.Failure;
                response.Message = e.Message;
                return BadRequest(response);
            } catch (Exception e) {
                response.Status = ResponseMessages.Failure;
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

                    response.Status = ResponseMessages.Success;
                    response.Message = ResponseMessages.SucessfullAction("Password", "deleted"); 
                    return Ok(response);
                } else {
                    response.Status = ResponseMessages.Failure;
                    response.Message = ResponseMessages.Unauthorized;
                    return Unauthorized(response);
                }
            } catch (InvalidEntityException e) {
                response.Status = ResponseMessages.Failure;
                response.Message = e.Message;
                return BadRequest(response);
            } catch (Exception e) {
                response.Status = ResponseMessages.Failure;
                response.Message = e.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

    }
}