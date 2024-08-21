using System.Linq.Expressions;
using PasswordManager.DTOs;
using PasswordManager.Models;

namespace PasswordManager.Services {
    public interface IUserService {
        Task<UserCreatedDto> AddUser(UserCreationDto user);

    }
}