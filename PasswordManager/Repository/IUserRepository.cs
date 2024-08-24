using System.Linq.Expressions;
using PasswordManager.Models;

namespace PasswordManager.Repository
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(int id);
        Task<User> AddAsync(User entity);

        Task<User> GetByValueAsync(Expression<Func<User, bool>> predicate);
    }
}