using Application.DTOs.Common;
using Application.DTOs.Tickets;
using Application.Enums;
using Application.Interfaces.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly AppDbContext _dbContext;

    public TicketRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<TicketSummaryDto>> GetPagedAsync(Guid? authorScope, TicketFilterDto filter, CancellationToken ct = default)
    {
        IQueryable<Ticket> query = _dbContext.Tickets.AsNoTracking();

        if (authorScope.HasValue)
            query = query.Where(t => t.AuthorId == authorScope.Value);

        if (filter.Status.HasValue)
            query = query.Where(t => t.Status == filter.Status);

        if (!string.IsNullOrWhiteSpace(filter.SearchQuery))
        {
            var terms = filter.SearchQuery
                .ToLower()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var term in terms)
            {
                query = query.Where(t =>
                    t.Title.ToLower().Contains(term) ||
                    t.Author.Login.ToLower().Contains(term));
            }
        }

        query = filter.SortBy switch
        {
            TicketSortBy.LastActivityAt => filter.IsDescending
                    ? query.OrderByDescending(t => t.LastActivityAt)
                    : query.OrderBy(t => t.LastActivityAt),

            _ => filter.IsDescending
                    ? query.OrderByDescending(t => t.CreatedAt)
                    : query.OrderBy(t => t.CreatedAt)
        };

        var total = await query.CountAsync(ct);
        
        var skip = (filter.Page - 1) * filter.PageSize;

        var items = await query
            .Skip(skip)
            .Take(filter.PageSize)
            .Select(t => new TicketSummaryDto
            {
                Id = t.Id,
                Title = t.Title,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
                LastActivityAt = t.LastActivityAt
            })
            .ToListAsync(ct);

        return new PagedResult<TicketSummaryDto>
        {
            Items = items,
            TotalCount = total,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<Ticket?> GetByIdAsync(Guid ticketId, CancellationToken ct = default, bool asNoTracking = false)
    {
        IQueryable<Ticket> query = _dbContext.Tickets;

        if (asNoTracking)
            query = query.AsNoTracking();

        return await query
            .Include(t => t.Author)
            .Include(t => t.Comments)
            .ThenInclude(c => c.Author)
            .FirstOrDefaultAsync(t => t.Id == ticketId, ct);
    }

    public async Task AddAsync(Ticket ticket, CancellationToken ct = default)
    {
        await _dbContext.Tickets.AddAsync(ticket, ct);
    }
}