using TheBoard.Core.Models;

namespace TheBoard.Application.Stores;

public interface IUserRepository
{
    Task Add(User user);
    Task Update(Guid id, string email, string passwordHash, string username);
    Task Delete(Guid id);
    Task<User> GetById(Guid id);
    Task<User> GetByEmail(string email);
}