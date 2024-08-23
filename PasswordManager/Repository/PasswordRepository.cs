using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PasswordManager.Data;
using PasswordManager.Models;

namespace PasswordManager.Repository
{
    public class PasswordRepository : IPasswordRepository {
               private readonly PasswordManagerContext _dbContext;
        private readonly DbSet<Password> _dbSet;

        public PasswordRepository(PasswordManagerContext context) {
            _dbContext = context;
            _dbSet = context.Set<Password>();
        }

        public async Task<List<Password>> GetAllAsync() {
            return await _dbSet.ToListAsync();
        }

        public async Task<Password> GetByIdAsync(int id) {
            return await _dbSet.FindAsync(id);
        }

        public async Task<Password> AddAsync(Password entity) {
            await _dbSet.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public async Task<Password> UpdateAsync(Password entity) {
            _dbSet.Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(Password entity) {
            _dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Password> GetByValueAsync(Expression<Func<Password, bool>> predicate) {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<Password>> GetAllByValueAsync(int userId) {
            return await _dbContext.Passwords.Where(p => p.UserId == userId).ToListAsync();
        }
    }
}