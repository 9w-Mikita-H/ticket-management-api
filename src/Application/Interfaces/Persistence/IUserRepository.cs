using Domain.Entities;

namespace Application.Interfaces.Persistence;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default, bool asNoTracking = false);

    Task<User?> GetByLoginAsync(string login, CancellationToken ct = default, bool asNoTracking = false);

    Task<bool> ExistsByLoginAsync(string login, CancellationToken ct = default);

    Task AddAsync(User user, CancellationToken ct = default);
}
