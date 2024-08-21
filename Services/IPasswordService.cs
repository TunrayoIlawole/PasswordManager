using PasswordManager.Models;

namespace PasswordManager.Services {
    public interface IPasswordService {
        Task<List<PasswordDto>> GetPasswords(string token);
        Task<PasswordFullDto> GetPassword(int id, string token);
        Task<PasswordDto> AddPassword(PasswordCreationDto entity, string token);
        Task<PasswordDto> UpdatePassword(int id, PasswordCreationDto entity);
        Task DeletePassword(int id);

        // Task<Password> GetByValueAsync(Expression<Func<Password, bool>> predicate);

        // Task<IEnumerable<Password>> GetAllByValueAsync(int userId);
    }
}