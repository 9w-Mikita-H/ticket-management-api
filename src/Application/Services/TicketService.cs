using Application.DTOs.Common;
using Application.DTOs.Tickets;
using Application.DTOs.Comments;
using Application.Interfaces.Persistence;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _tickets;
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;

    public TicketService(ITicketRepository tickets, IUserRepository users, IUnitOfWork uow)
    {
        _tickets = tickets;
        _users = users;
        _uow = uow;
    }

    public async Task<PagedResult<TicketListItemDto>> GetAsync(
        Guid currentUserId,
        UserRole role,
        TicketFilterDto filter,
        CancellationToken ct = default)
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

            "CreatedAt" =>
                filter.SortDirection == "desc"
                    ? query.OrderByDescending(t => t.CreatedAt)
                    : query.OrderBy(t => t.CreatedAt)
        };

        var total = query.Count();

        var items = query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(t => new TicketListItemDto
            {
                Id = t.Id,
                Title = t.Title,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
                LastActivityAt = t.LastActivityAt
            })
            .ToList();

        return new PagedResult<TicketListItemDto>
        {
            Items = items,
            TotalCount = total,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<TicketDetailsDto?> GetByIdAsync(
        Guid ticketId,
        Guid currentUserId,
        UserRole role,
        CancellationToken ct = default)
    {
        var ticket = await _tickets.GetByIdAsync(ticketId, ct);

        if (ticket is null)
            return null;

        if (role == UserRole.User && ticket.AuthorId != currentUserId)
            throw new UnauthorizedAccessException();

        return new TicketDetailsDto
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            Status = ticket.Status,
            CreatedAt = ticket.CreatedAt,
            LastActivityAt = ticket.LastActivityAt,
            AuthorLogin = ticket.Author.Login,
            Comments = ticket.Comments.OrderBy(c => c.CreatedAt)
                .Select(c => new CommentResponseDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    AuthorLogin = c.Author.Login,
                    CreatedAt = c.CreatedAt
                })
                .ToList()
        };
    }

    public async Task<Guid> CreateAsync(
        Guid currentUserId,
        CreateTicketDto dto,
        CancellationToken ct = default)
    {
        var author = await _users.GetByIdAsync(currentUserId, ct)
                     ?? throw new InvalidOperationException();

        var ticket = new Ticket(dto.Title, dto.Description, author);

        await _tickets.AddAsync(ticket, ct);
        await _uow.SaveChangesAsync(ct);

        return ticket.Id;
    }

    public async Task UpdateAsync(
        Guid ticketId,
        Guid currentUserId,
        UpdateTicketDto dto,
        CancellationToken ct = default)
    {
        var ticket = await _tickets.GetByIdAsync(ticketId, ct)
                     ?? throw new InvalidOperationException();

        if (ticket.AuthorId != currentUserId)
            throw new UnauthorizedAccessException();

        if (ticket.Status != TicketStatus.Open)
            throw new InvalidOperationException();

        if (ticket.Comments.Any(c => c.Author.Role == UserRole.Agent))
            throw new InvalidOperationException();

        ticket.Title = dto.Title;
        ticket.Description = dto.Description;
        ticket.LastActivityAt = DateTime.UtcNow;

        await _uow.SaveChangesAsync(ct);
    }

    public async Task AddCommentAsync(
        Guid ticketId,
        Guid currentUserId,
        UserRole role,
        AddCommentDto dto,
        CancellationToken ct = default)
    {
        var ticket = await _tickets.GetByIdAsync(ticketId, ct)
                     ?? throw new InvalidOperationException();

        if (ticket.Status == TicketStatus.Closed)
            throw new InvalidOperationException();

        if (role == UserRole.User && ticket.AuthorId != currentUserId)
            throw new UnauthorizedAccessException();

        var author = await _users.GetByIdAsync(currentUserId, ct)
                     ?? throw new InvalidOperationException();

        ticket.Comments.Add(new Comment(dto.Content, ticket, author));
        ticket.LastActivityAt = DateTime.UtcNow;

        await _uow.SaveChangesAsync(ct);
    }

    public async Task ChangeStatusAsync(
        Guid ticketId,
        Guid currentUserId,
        UserRole role,
        TicketStatus newStatus,
        CancellationToken ct = default)
    {
        var ticket = await _tickets.GetByIdAsync(ticketId, ct)
                     ?? throw new InvalidOperationException();

        switch (role)
        {
            case UserRole.User:
                if (ticket.AuthorId != currentUserId || newStatus != TicketStatus.Closed)
                    throw new UnauthorizedAccessException();

                if (ticket.Status is not (TicketStatus.Open or TicketStatus.Resolved))
                    throw new InvalidOperationException();
                break;
            
            case UserRole.Agent:
                var allowed = (ticket.Status, newStatus) switch
                {
                    (TicketStatus.Open, TicketStatus.InProgress) => true,
                    (TicketStatus.InProgress, TicketStatus.Resolved) => true,
                    (TicketStatus.InProgress, TicketStatus.Open) => true,
                    (TicketStatus.Resolved, TicketStatus.InProgress) => true,
                    (TicketStatus.Resolved, TicketStatus.Closed) => true,
                    _ => false
                };

                if (!allowed)
                    throw new InvalidOperationException();
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(role));
        }

        ticket.Status = newStatus;
        ticket.LastActivityAt = DateTime.UtcNow;

        await _uow.SaveChangesAsync(ct);
    }
}