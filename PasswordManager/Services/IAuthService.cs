using PasswordManager.Models.DTOs;

namespace PasswordManager.Services {
public interface IAuthService {
    Task<string> SignInUser(LoginDto loginDto);
    
}
}