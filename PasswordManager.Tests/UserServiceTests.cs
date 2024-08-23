using Moq;
using PasswordManager.Models;
using PasswordManager.Models.DTOs;
using PasswordManager.Repository;
using PasswordManager.Services;

namespace PasswordManager.Tests {
    public class UserServiceTests {
        private readonly UserService _userService;
        private readonly Mock<IUserRepository> _mockRepository;

        public UserServiceTests() {
            _mockRepository = new Mock<IUserRepository>();
            _userService = new UserService(_mockRepository.Object);
        }

        [Fact]
        public async Task AddUser_ShouldCreateUser() {
            var userDto = new UserCreationDto {
                Email = "test@gmail.com",
                Username = "testuser",
                Password = "testpassword123"
            };
            setupMockRepository(userDto);

            var result = await _userService.AddUser(userDto);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("test@gmail.com", result.Email);

        }

        public void setupMockRepository(UserCreationDto userDto) {
            _mockRepository.Setup(repo => repo.AddAsync(It.IsAny<User>()))
            .ReturnsAsync(new User {
                Id = 1,
                Email = userDto.Email,
                Username = userDto.Username
            });
        }
    }
}