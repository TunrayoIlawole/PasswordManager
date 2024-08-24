namespace PasswordManager.Models.DTOs {
    public class PasswordDto {
        public required int Id { get; set; }
        public required string WebsiteUrl { get; set; }
        public required string EmailOrUsername { get; set; }

        public required int UserId { get; set; }
    }
}