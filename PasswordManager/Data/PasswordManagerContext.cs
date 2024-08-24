using Microsoft.EntityFrameworkCore;
using PasswordManager.Models;

namespace PasswordManager.Data
{
    public class PasswordManagerContext : DbContext {
        public PasswordManagerContext(DbContextOptions<PasswordManagerContext> options) : base(options) {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Password> Passwords { get; set; }

    }
}