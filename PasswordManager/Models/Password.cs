using System.ComponentModel.DataAnnotations;

namespace PasswordManager.Models
{
    public class Password {
        [Key]
        public int Id { get; set; }

        [Required]
        public string WebsiteUrl { get; set; }

        [Required]
        public string EmailOrUsername { get; set; }

        [Required]
        public string WebsitePassword { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        

        
    }
}