using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Npgsql.Internal;
using PasswordManager.Models;
using PasswordManager.Repository;

namespace PasswordManager.Services {

    public class PasswordService : IPasswordService {

        private readonly IPasswordRepository _passwordRepository;

        public PasswordService(IPasswordRepository passwordRepository) {
            _passwordRepository = passwordRepository;
        }

        public async Task<PasswordDto> AddPassword(PasswordCreationDto passwordDto, string token) {
            int userId = decodeJWT(token);
            Aes aes = Aes.Create();
            byte[] encrypted = EncryptPassword(passwordDto.WebsitePassword, aes.Key, aes.IV);
            String encryptedPassword = Convert.ToBase64String(encrypted);
            Password newPassword = new()
                {
                    EmailOrUsername = passwordDto.EmailOrUsername,
                    WebsiteUrl = passwordDto.WebsiteUrl,
                    WebsitePassword = encryptedPassword,
                    UserId = userId
                };
            
            var password = await _passwordRepository.AddAsync(newPassword);

            return new PasswordDto {
                WebsiteUrl = password.WebsiteUrl,
                EmailOrUsername = password.EmailOrUsername,
                UserId = userId
            };
        }

        public async Task<List<PasswordDto>> GetPasswords(string token) {
            var userId = decodeJWT(token);

            var passwordsResult = await _passwordRepository.GetAllByValueAsync(userId);

            List<PasswordDto> passwords = new List<PasswordDto>();

            foreach (Password password in passwordsResult)
            {
                passwords.Add(new PasswordDto{
                    EmailOrUsername = password.EmailOrUsername,
                    WebsiteUrl = password.WebsiteUrl,
                    UserId = password.UserId
                });
            };

            return passwords;

        }

        public async Task<PasswordFullDto> GetPassword(int id, string token) {
            int userId = decodeJWT(token);

            var password = await _passwordRepository.GetByIdAsync(id);

            Aes aes = Aes.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(password.WebsitePassword);
            string decrypted = DecryptPassword(bytes, aes.Key, aes.IV);

            if (password == null || userId != password.UserId) {
                throw new InvalidEntityException("Password not found");
            }

            return new PasswordFullDto {
                EmailOrUsername = password.EmailOrUsername,
                WebsiteUrl = password.WebsiteUrl,
                WebsitePassword = decrypted,
                UserId = password.UserId

            };
        }

        public async Task<PasswordDto> UpdatePassword(int id, PasswordCreationDto passwordDto) {
            var existingPassword = await _passwordRepository.GetByIdAsync(id) ?? throw new InvalidEntityException("Password " + id + " does not exist");

            existingPassword.EmailOrUsername = passwordDto.EmailOrUsername;
            existingPassword.WebsiteUrl = passwordDto.WebsiteUrl;
            existingPassword.WebsitePassword = passwordDto.WebsitePassword;

            Password updatedPassword = await _passwordRepository.UpdateAsync(existingPassword);

            return new PasswordDto {
                EmailOrUsername = updatedPassword.EmailOrUsername,
                WebsiteUrl = updatedPassword.WebsiteUrl,
                UserId = updatedPassword.UserId
            };
        }

        public async Task DeletePassword(int id) {
            var existingPassword = await _passwordRepository.GetByIdAsync(id) ?? throw new InvalidEntityException("Password " + id + " does not exist");

            await _passwordRepository.DeleteAsync(existingPassword);
        }


        private int decodeJWT(string token) {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var userIdClaim = jwtToken.Claims.First(claim => claim.Type == "userId");

            if (userIdClaim == null) {
                throw new InvalidOperationException("The 'userId' claim is missing");
            }
            var userId = userIdClaim.Value;

            return int.Parse(userId);
            
        }

        private static byte[] EncryptPassword(String password, byte[] key, byte[] iv) {
            byte[] encrypted;

            using (Aes aes = Aes.Create()) {
                aes.Key = key;
                aes.IV = iv;

                using (MemoryStream memoryStream = new MemoryStream()) {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write)) {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream)) {
                            streamWriter.Write(password);
                        }
                        encrypted = memoryStream.ToArray();
                    }
                }
            }

            return encrypted;
        }

        private static string DecryptPassword(byte[] encodedText, byte[] key, byte[] iv) {
            string decrypted;

            using (Aes aes = Aes.Create()) {
                aes.Key = key;
                aes.IV = iv;

                using (MemoryStream memoryStream = new MemoryStream(encodedText)) {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read)) {
                        using (StreamReader streamReader = new StreamReader(cryptoStream)) {
                            decrypted = streamReader.ReadToEnd();
                        }
                    }
                }
            }

            return decrypted;
        }


    }

}