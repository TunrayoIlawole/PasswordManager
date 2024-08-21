using PasswordManager.DTOs;
using PasswordManager.Models;

namespace PasswordManager.Services {
public interface IAuthService {
    Task<string> SignInUser(LoginDto loginDto);
    
}
}