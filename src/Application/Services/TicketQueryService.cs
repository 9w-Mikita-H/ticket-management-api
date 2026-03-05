using Application.DTOs.Common;
using Application.DTOs.Tickets;
using Application.Interfaces.Persistence;
using Application.Interfaces.Services;
using Application.Mappings;
using Domain.Enums;
using Domain.Services;

namespace Application.Services;

public class TicketQueryService : ITicketQueryService
{
    private readonly ITicketRepository _tickets;
    private readonly ITicketDomainService _ticketDomainService;

    public TicketQueryService(ITicketRepository tickets, ITicketDomainService ticketDomainService)
    {
        _tickets = tickets;
        _ticketDomainService = ticketDomainService;
    }

    public Task<PagedResult<TicketSummaryDto>> GetAsync(Guid currentUserId, UserRole role, TicketFilterDto filter, CancellationToken ct = default)
    {
        Guid? authorScope = role is UserRole.User ? currentUserId : null;

        return _tickets.GetPagedAsync(authorScope, filter, ct);
    }

    public async Task<TicketDetailsDto?> GetByIdAsync(Guid ticketId, Guid currentUserId, UserRole role, CancellationToken ct = default)
    {
        var ticket = await _tickets.GetByIdAsync(ticketId, ct, asNoTracking: true);

        if (ticket is null)
            return null;

        _ticketDomainService.EnsureCanView(ticket, currentUserId, role);

        return ticket.ToDetailsDto();
    }
}