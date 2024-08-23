using PasswordManager.Models.DTOs;
using PasswordManager.Models;
using PasswordManager.Repository;
using PasswordManager.Responses;

namespace PasswordManager.Services {

    public class UserService : IUserService {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository) {
            _userRepository = userRepository;
        }

        public async Task<UserCreatedDto> AddUser(UserCreationDto userDto) {
            var existingUser = await _userRepository.GetByValueAsync(u => u.Email == userDto.Email);

            if (existingUser != null) {
                throw new DuplicateEntityException(ResponseMessages.DuplicateUser(userDto.Email));
            }

            User newUser = new()
            {
                Email = userDto.Email,
                Username = userDto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password)
            };

            var user = await _userRepository.AddAsync(newUser);

            return new UserCreatedDto {
                Id = user.Id, 
                Email = user.Email,
                Username = user.Username
            };

        }

        
        
    }
    
}