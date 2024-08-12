using Microsoft.EntityFrameworkCore;
using TheBoard.Application.Stores;
using TheBoard.Core.Models;

namespace TheBoard.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly TheBoardDbContext _context;

    public UserRepository(TheBoardDbContext context)
    {
        _context = context;
    }

    public async Task Add(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        await _context.Users
            .Where(u => u.Id == id)
            .ExecuteDeleteAsync();
    }

    public async Task<User> GetByEmail(string email)
    {
        var result = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);
        return result;
    }

    public async Task<User> GetById(Guid id)
    {
        var result = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
        return result;
    }

    public async Task Update(Guid id, string email, string passwordHash, string username)
    {
        await _context.Users
            .Where(u => u.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(u => u.Email, email)
                .SetProperty(u => u.Username, username)
                .SetProperty(u => u.PasswordHash, passwordHash));
    }
}
