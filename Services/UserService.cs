using PasswordManager.DTOs;
using PasswordManager.Models;
using PasswordManager.Repository;

namespace PasswordManager.Services {

    public class UserService : IUserService {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordRepository _passwordRepository;

        public UserService(IUserRepository userRepository, IPasswordRepository passwordRepository) {
            _userRepository = userRepository;
            _passwordRepository = passwordRepository;
        }

        public async Task<UserCreatedDto> AddUser(UserCreationDto userDto) {
            var existingUser = await _userRepository.GetByValueAsync(u => u.Email == userDto.Email);

            if (existingUser != null) {
                throw new DuplicateEntityException("User with email " + userDto.Email + " already exists");
            }

            User newUser = new()
            {
                Email = userDto.Email,
                Username = userDto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password)
            };

            var user = await _userRepository.AddAsync(newUser);

            return new UserCreatedDto {
                Email = user.Email,
                Username = user.Username
            };

        }

        
        
    }
    
}