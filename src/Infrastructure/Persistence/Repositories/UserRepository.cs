using Application.Interfaces.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default, bool asNoTracking = false)
    {
        IQueryable<User> query = _dbContext.Users;

        if (asNoTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<User?> GetByLoginAsync(string login, CancellationToken ct = default, bool asNoTracking = false)
    {
        IQueryable<User> query = _dbContext.Users;

        if (asNoTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(u => u.Login == login, ct);
    }

    public async Task<bool> ExistsByLoginAsync(string login, CancellationToken ct = default)
    {
        return await _dbContext.Users.AnyAsync(u => u.Login == login, ct);
    }

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await _dbContext.Users.AddAsync(user, ct);
    }
}