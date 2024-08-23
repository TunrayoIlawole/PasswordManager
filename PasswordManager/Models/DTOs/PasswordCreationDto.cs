namespace PasswordManager.Models.DTOs {
    public class PasswordCreationDto {
        public required string WebsiteUrl { get; set; }
        public required string EmailOrUsername { get; set; }
        public required string WebsitePassword { get; set; }
    }
}