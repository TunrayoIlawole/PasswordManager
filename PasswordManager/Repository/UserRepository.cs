using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Data;
using PasswordManager.Models;

namespace PasswordManager.Repository
{
    public class UserRepository : IUserRepository {

        private readonly PasswordManagerContext _dbContext;
        private readonly DbSet<User> _dbSet;

        public UserRepository(PasswordManagerContext context) {
            _dbContext = context;
            _dbSet = context.Set<User>();
        }

        public async Task<IEnumerable<User>> GetAllAsync() {
            return await _dbSet.ToListAsync();
        }

        public async Task<User> GetByIdAsync(int id) {
            return await _dbSet.FindAsync(id);
        }

        public async Task<User> AddAsync(User entity) {
            await _dbSet.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public async Task<User> GetByValueAsync(Expression<Func<User, bool>> predicate) {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }
    }
}