namespace PasswordManager.Models.DTOs {

    public class UserCreatedDto {
        public required int Id { get; set; }
        public required string Email  { get; set; }
        public required string Username  { get; set; }
        
    }
}