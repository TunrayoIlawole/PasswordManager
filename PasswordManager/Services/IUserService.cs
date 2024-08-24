using System.Linq.Expressions;
using PasswordManager.Models.DTOs;
using PasswordManager.Models;

namespace PasswordManager.Services {
    public interface IUserService {
        Task<UserCreatedDto> AddUser(UserCreationDto user);

    }
}