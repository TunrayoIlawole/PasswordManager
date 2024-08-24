using Microsoft.AspNetCore.Mvc;
using PasswordManager.Models.DTOs;
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

        public UserController(IUserService userService, IPasswordService passwordService) {
            _userService = userService;
            _passwordService = passwordService;
        }

        [HttpPost]
        public async Task<IActionResult> PostUserAsync(UserCreationDto data) 
        {
            ResponseData<UserCreatedDto> response = new ResponseData<UserCreatedDto>();
            try {
                UserCreatedDto user = await _userService.AddUser(data);
                response.Status = ResponseMessages.Success;
                response.Message = ResponseMessages.SucessfullAction("User", "created"); 
                response.Data = user;

                return Created(user.Id.ToString(), response);
            } catch (DuplicateEntityException e) {
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
        [HttpGet("{userId}/passwords")]
        public async Task<IActionResult> GetPasswords(int userId) {

            ResponseData<List<PasswordDto>> response = new ResponseData<List<PasswordDto>>();

            try {
                var authHeader = Request.Headers["Authorization"].ToString();

                if (authHeader != null && authHeader.StartsWith("Bearer ")) {
                    var token = authHeader.Substring("Bearer ".Length).Trim();
                    
                    List<PasswordDto> passwords = await _passwordService.GetPasswords(userId, token);

                    response.Status = ResponseMessages.Success;
                    response.Message = ResponseMessages.SucessfullAction("User passwords", "retrieved"); 
                    response.Data = passwords;
                    return Ok(response);

                } else {
                    response.Status = ResponseMessages.Failure;
                    response.Message = ResponseMessages.Unauthorized;
                    return Unauthorized(response);
                }
            } catch (InvalidEntityException e) {
                    response.Status = ResponseMessages.Failure;
                    response.Message = e.Message;
                    return Unauthorized(response);
            } catch (Exception e) {
                response.Status = ResponseMessages.Failure;
                response.Message = e.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

    }
}