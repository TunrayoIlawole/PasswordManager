using Microsoft.Extensions.Configuration;
using Moq;
using PasswordManager.Models;
using PasswordManager.Models.DTOs;
using PasswordManager.Repository;
using PasswordManager.Services;
using System;
using System.Linq.Expressions;

namespace PasswordManager.Tests {

    public class AuthServiceTests {
        private readonly AuthService _authService;
        private readonly Mock <IUserRepository> _mockRepository;

        private readonly Mock<IConfiguration> _mockConfiguration;

        public AuthServiceTests() {
            _mockRepository = new Mock<IUserRepository>();
            _mockConfiguration = new Mock<IConfiguration>();
            _authService = new AuthService(_mockRepository.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task   SignInUser_ShouldReturnAuthToken() {

            LoginDto loginDto = new LoginDto {
                Email = "test@gmail.com",
                Password = "testpassword123"
            };

            var user = new User {
                Id = 1,
                Email = loginDto.Email,
                Username = "testuser",
                Password = BCrypt.Net.BCrypt.HashPassword(loginDto.Password)
            };

            _mockRepository
            .Setup(repo => repo.GetByValueAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(user);

            var token = _authService.SignInUser(loginDto);

            Assert.NotNull(token);
            
        }

        [Fact]
        public async Task SignInUser_ShouldReturnUnauthorizedAccessExceptionWhenPasswordIsInvalid() {

            LoginDto loginDto = new LoginDto {
                Email = "test@gmail.com",
                Password = "incorrectPassword"
            };

            var user = new User {
                Id = 1,
                Email = loginDto.Email,
                Username = "testuser",
                Password = BCrypt.Net.BCrypt.HashPassword("correctPassword")
            };

            _mockRepository
            .Setup(repo => repo.GetByValueAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(user);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.SignInUser(loginDto));
            
        }

        [Fact]
        public async Task SignInUser_ShouldThrowInvalidEntityExceptionIfUserNotFound() {
            LoginDto loginDto = new LoginDto {
                Email = "notfound@gmail.com",
                Password = "testpassword"
            };

            _mockRepository
            .Setup(repo => repo.GetByValueAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync((User)null);

            await Assert.ThrowsAsync<InvalidEntityException>(() => _authService.SignInUser(loginDto));
        }

    }
}