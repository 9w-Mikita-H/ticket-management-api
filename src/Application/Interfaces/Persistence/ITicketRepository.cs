using Domain.Entities;

namespace Application.Interfaces.Persistence;

public interface ITicketRepository
{
    Task<Ticket?> GetByIdAsync(Guid ticketId, CancellationToken cancellationToken = default);

    Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid ticketId, CancellationToken cancellationToken = default);

    IQueryable<Ticket> Query();
}