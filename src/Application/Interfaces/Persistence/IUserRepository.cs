using Domain.Entities;

namespace Application.Interfaces.Persistence;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    
    Task<User?> GetByLoginAsync(string login, CancellationToken cancellationToken = default);

    Task<bool> ExistsByLoginAsync(string login, CancellationToken cancellationToken = default);

    Task AddAsync(User user, CancellationToken cancellationToken = default);
}