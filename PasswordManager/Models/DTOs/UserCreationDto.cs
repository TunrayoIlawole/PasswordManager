namespace PasswordManager.Models.DTOs {

    public class UserCreationDto {

        public required string Email  { get; set; }
        public required string Username  { get; set; }
        public required string Password  { get; set; }
        
    }
}