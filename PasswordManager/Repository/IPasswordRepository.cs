using System.Linq.Expressions;
using PasswordManager.Models;

namespace PasswordManager.Repository
{
    public interface IPasswordRepository
    {
        Task<List<Password>> GetAllAsync();
        Task<Password> GetByIdAsync(int id);
        Task<Password> AddAsync(Password entity);
        Task<Password> UpdateAsync(Password entity);
        Task DeleteAsync(Password entity);

        Task<Password> GetByValueAsync(Expression<Func<Password, bool>> predicate);

        Task<IEnumerable<Password>> GetAllByValueAsync(int userId);
    }
}