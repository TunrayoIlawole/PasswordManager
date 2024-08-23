using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Moq;
using PasswordManager.Models;
using PasswordManager.Models.DTOs;
using PasswordManager.Repository;
using PasswordManager.Services;

namespace PasswordManager.Tests {

    public class PasswordServiceTests {
        private readonly Mock<IPasswordRepository> _mockPasswordRepository;
        private readonly Mock<IEncryptionService> _mockEncryptionService;

        private readonly PasswordService _passwordService;

        public PasswordServiceTests() {
            _mockPasswordRepository = new Mock<IPasswordRepository>();
            _mockEncryptionService = new Mock<IEncryptionService>();
            _passwordService = new PasswordService(_mockPasswordRepository.Object, _mockEncryptionService.Object);
        }

        [Fact]
        public async Task AddPassword_ShouldAddPassword() {

            PasswordCreationDto passwordCreationDto = new PasswordCreationDto {
                EmailOrUsername = "testUsername7",
                WebsiteUrl = "https://animefandom.com",
                WebsitePassword = "password123"
            };

            var token = GenerateToken(1);

            _mockEncryptionService.Setup(service => service.EncryptPassword(passwordCreationDto.WebsitePassword))
            .Returns("encryptedPassword");

            Password password = new Password {
                Id = 1,
                EmailOrUsername = passwordCreationDto.EmailOrUsername,
                WebsiteUrl = passwordCreationDto.WebsiteUrl,
                WebsitePassword = "encryptedPassword",
                UserId = 1
            };

            _mockPasswordRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Password>()))
            .ReturnsAsync(password);

            var result = await _passwordService.AddPassword(passwordCreationDto, token);

            Assert.NotNull(result);
            Assert.Equal(password.Id, result.Id);
            Assert.Equal(passwordCreationDto.EmailOrUsername, result.EmailOrUsername);

        }

        [Fact]
        public async Task GetPasswords_ShouldReturnPasswordsIfUserIsValid() {
            var token = GenerateToken(1);
            var passwords = new List<Password> {
                new Password { Id = 1, EmailOrUsername = "testUsername7", WebsiteUrl ="https://animefandom.com", WebsitePassword = "encryptedPassword", UserId = 1 }
            };

            _mockPasswordRepository
                .Setup(repo => repo.GetAllByValueAsync(1))
                .ReturnsAsync(passwords);

            var result = await _passwordService.GetPasswords(1, token);

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetPassword_ShouldShowPasswordIfUserIsValid() {
            var token = GenerateToken(1);
            var password = new Password {
                Id = 1,
                EmailOrUsername = "testUsername7",
                WebsiteUrl = "https://animefandom.com",
                WebsitePassword = "encryptedPassword",
                UserId = 1
            };

            _mockPasswordRepository
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(password);

            _mockEncryptionService
                .Setup(e => e.DecryptPassword(password.WebsitePassword))
                .Returns("password123");

            
            var result = await _passwordService.GetPassword(1, token);

            Assert.NotNull(result);
            Assert.Equal(password.EmailOrUsername, result.EmailOrUsername);
            Assert.Equal("password123", result.WebsitePassword);

        }

        private string GenerateToken(int userId) {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("VeryLongAndSecureSecretKey1234567890"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new[] {
                    new Claim("userId", userId.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = "Test_Issuer",
                Audience = "Test_Audience",
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


    }
}