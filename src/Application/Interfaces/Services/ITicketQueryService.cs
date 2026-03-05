using Application.DTOs.Common;
using Application.DTOs.Tickets;
using Domain.Enums;

namespace Application.Interfaces.Services;

public interface ITicketQueryService
{
    Task<PagedResult<TicketSummaryDto>> GetAsync(Guid currentUserId, UserRole currentUserRole, TicketFilterDto filter, CancellationToken ct = default);

    Task<TicketDetailsDto?> GetByIdAsync(Guid ticketId, Guid currentUserId, UserRole currentUserRole, CancellationToken ct = default);
}