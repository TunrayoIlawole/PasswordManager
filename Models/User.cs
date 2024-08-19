using System.ComponentModel.DataAnnotations;

namespace PasswordManager.Models
{
    public class User {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public ICollection<Password> passwords { get; set; }



        
    }
}