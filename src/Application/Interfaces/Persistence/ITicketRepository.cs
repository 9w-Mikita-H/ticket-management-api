using Application.DTOs.Common;
using Application.DTOs.Tickets;
using Domain.Entities;

namespace Application.Interfaces.Persistence;

public interface ITicketRepository
{
    Task<Ticket?> GetByIdAsync(Guid ticketId, CancellationToken ct = default, bool asNoTracking = false);

    Task AddAsync(Ticket ticket, CancellationToken ct = default);

    Task<PagedResult<TicketSummaryDto>> GetPagedAsync(Guid? authorScope, TicketFilterDto filter, CancellationToken ct = default);
}