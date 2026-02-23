using Application.DTOs.Comments;
using Application.DTOs.Common;
using Application.DTOs.Tickets;
using Application.Interfaces.Persistence;
using Application.Interfaces.Services;
using Application.Mappings;
using Domain.Enums;

namespace Application.Services;

public class TicketQueryService : ITicketQueryService
{
    private readonly ITicketRepository _tickets;
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;

    public TicketQueryService(ITicketRepository tickets, IUserRepository users, IUnitOfWork uow)
    {
        _tickets = tickets;
        _users = users;
        _uow = uow;
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
            "LastActivityAt" =>
                filter.SortDirection == "desc"
                    ? query.OrderByDescending(t => t.LastActivityAt)
                    : query.OrderBy(t => t.LastActivityAt),

            _ =>
                filter.SortDirection == "desc"
                    ? query.OrderByDescending(t => t.CreatedAt)
                    : query.OrderBy(t => t.CreatedAt)
        };

        var total = query.Count();

        var items = query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(t => t.ToSummaryDto())
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

        if (role == UserRole.User && ticket.AuthorId != currentUserId)
            throw new UnauthorizedAccessException();

        return ticket.ToDetailsDto();
    }
}