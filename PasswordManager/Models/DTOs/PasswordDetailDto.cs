namespace PasswordManager.Models.DTOs {
    public class PasswordDetailDto {
        public required string WebsiteUrl { get; set; }
        public required string EmailOrUsername { get; set; }

        public required string WebsitePassword { get; set; }

        public required int UserId { get; set; }
    }
}