using Application.DTOs.Common;
using Application.DTOs.Tickets;
using Application.Enums;
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

    public async Task<PagedResult<TicketSummaryDto>> GetAsync(Guid currentUserId, UserRole role, TicketFilterDto filter, CancellationToken ct = default)
    {
        var query = _tickets.Query();

        if (role == UserRole.User)
            query = query.Where(t => t.AuthorId == currentUserId);

        if (filter.Status.HasValue)
            query = query.Where(t => t.Status == filter.Status);

        if (!string.IsNullOrWhiteSpace(filter.SearchQuery))
        {
            var q = filter.SearchQuery.ToLower();

            query = query.Where(t => t.Title.ToLower().Contains(q) || t.Author.Login.ToLower().Contains(q));
        }

        query = filter.SortBy switch
        {
            TicketSortBy.LastActivityAt =>
                filter.IsDescending
                    ? query.OrderByDescending(t => t.LastActivityAt)
                    : query.OrderBy(t => t.LastActivityAt),

            _ =>
                filter.IsDescending
                    ? query.OrderByDescending(t => t.CreatedAt)
                    : query.OrderBy(t => t.CreatedAt)
        };

        var total = query.Count();

        var items = query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(t => new TicketSummaryDto
            {
                Id = t.Id,
                Title = t.Title,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
                LastActivityAt = t.LastActivityAt
            })
            .ToList();

        return new PagedResult<TicketSummaryDto>
        {
            Items = items,
            TotalCount = total,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<TicketDetailsDto?> GetByIdAsync(Guid ticketId, Guid currentUserId, UserRole role, CancellationToken ct = default)
    {
        var ticket = await _tickets.GetByIdAsync(ticketId, ct);

        if (ticket is null)
            return null;

        _ticketDomainService.EnsureCanView(ticket, currentUserId, role);

        return ticket.ToDetailsDto();
    }
}
