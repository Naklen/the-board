using TheBoard.Application.Auth;
using TheBoard.Application.Stores;
using TheBoard.Core.Models;

namespace TheBoard.Application.Services;

public class UserService
{
    private readonly IUserRepository _userStore;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IUserRepository userStore, IPasswordHasher passwordHasher)
    {
        _userStore = userStore;
        _passwordHasher = passwordHasher;
    }

    public async Task Registrate(string userName, string email, string password)
    {
        var passwordHash = _passwordHasher.Hash(password);
        var user = new User()
        {
            Id = Guid.NewGuid(),
            Username = userName,
            Email = email,
            PasswordHash = passwordHash
        };
        await _userStore.Add(user);
    }
}
