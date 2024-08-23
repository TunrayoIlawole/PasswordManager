using PasswordManager.Models.DTOs;

namespace PasswordManager.Services {
    public interface IPasswordService {
        Task<List<PasswordDto>> GetPasswords(int userId, string token);
        Task<PasswordDetailDto> GetPassword(int id, string token);
        Task<PasswordDto> AddPassword(PasswordCreationDto entity, string token);
        Task<PasswordDto> UpdatePassword(int id, PasswordCreationDto entity, string token);
        Task DeletePassword(int id, string token);
    }
}