using System.IdentityModel.Tokens.Jwt;
using PasswordManager.Models;
using PasswordManager.Models.DTOs;
using PasswordManager.Repository;
using PasswordManager.Responses;

namespace PasswordManager.Services {

    public class PasswordService : IPasswordService {

        private readonly IPasswordRepository _passwordRepository;
        private readonly EncryptionService _encryptionService;

        public PasswordService(IPasswordRepository passwordRepository, EncryptionService encryptionService) {
            _passwordRepository = passwordRepository;
            _encryptionService = encryptionService;
        }

        public async Task<PasswordDto> AddPassword(PasswordCreationDto passwordDto, string token) {

            int userId = decodeJWT(token);

            string encryptedPassword = _encryptionService.EncryptPassword(passwordDto.WebsitePassword);
            Password newPassword = new()
                {
                    EmailOrUsername = passwordDto.EmailOrUsername,
                    WebsiteUrl = passwordDto.WebsiteUrl,
                    WebsitePassword = encryptedPassword,
                    UserId = userId
                };
            
            var password = await _passwordRepository.AddAsync(newPassword);

            return new PasswordDto {
                Id = password.Id,
                WebsiteUrl = password.WebsiteUrl,
                EmailOrUsername = password.EmailOrUsername,
                UserId = userId
            };
        }

        public async Task<List<PasswordDto>> GetPasswords(int userId, string token) {
            var derivedUserId = decodeJWT(token);
            Console.WriteLine("It got here");
            Console.WriteLine(derivedUserId);

            if (derivedUserId != userId) {
                throw new InvalidEntityException(ResponseMessages.InvalidPasswords);
            }

            var passwordsResult = await _passwordRepository.GetAllByValueAsync(derivedUserId);

            List<PasswordDto> passwords = new List<PasswordDto>();

            foreach (Password password in passwordsResult)
            {
                passwords.Add(new PasswordDto{
                    Id = password.Id,
                    EmailOrUsername = password.EmailOrUsername,
                    WebsiteUrl = password.WebsiteUrl,
                    UserId = password.UserId
                });
            };

            return passwords;

        }

        public async Task<PasswordDetailDto> GetPassword(int id, string token) {
            int userId = decodeJWT(token);

            var password = await _passwordRepository.GetByIdAsync(id);
            string decryptedPassword = _encryptionService.DecryptPassword(password.WebsitePassword);

            if (password == null || userId != password.UserId) {
                throw new InvalidEntityException(ResponseMessages.InvalidPassword);
            }

            return new PasswordDetailDto {
                EmailOrUsername = password.EmailOrUsername,
                WebsiteUrl = password.WebsiteUrl,
                WebsitePassword = decryptedPassword,
                UserId = password.UserId

            };
        }

        public async Task<PasswordDto> UpdatePassword(int id, PasswordCreationDto passwordDto, string token) {
            var existingPassword = await _passwordRepository.GetByIdAsync(id);
            int userId = decodeJWT(token);

            if (existingPassword == null || existingPassword.UserId != userId) {
                throw new InvalidEntityException(ResponseMessages.InvalidPassword);
            }

            existingPassword.EmailOrUsername = passwordDto.EmailOrUsername;
            existingPassword.WebsiteUrl = passwordDto.WebsiteUrl;
            existingPassword.WebsitePassword = passwordDto.WebsitePassword;

            Password updatedPassword = await _passwordRepository.UpdateAsync(existingPassword);

            return new PasswordDto {
                Id = updatedPassword.Id,
                EmailOrUsername = updatedPassword.EmailOrUsername,
                WebsiteUrl = updatedPassword.WebsiteUrl,
                UserId = updatedPassword.UserId
            };
        }

        public async Task DeletePassword(int id, string token) {
            var existingPassword = await _passwordRepository.GetByIdAsync(id);
            int userId = decodeJWT(token);

            if (existingPassword == null || existingPassword.UserId != userId) {
                throw new InvalidEntityException(ResponseMessages.InvalidPassword);
            }

            await _passwordRepository.DeleteAsync(existingPassword);
        }


        private int decodeJWT(string token) {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var userIdClaim = jwtToken.Claims.First(claim => claim.Type == "userId");

            if (userIdClaim == null) {
                throw new InvalidOperationException(ResponseMessages.InvalidClaim("userId"));
            }
            var userId = userIdClaim.Value;

            return int.Parse(userId);
            
        }


    }

}