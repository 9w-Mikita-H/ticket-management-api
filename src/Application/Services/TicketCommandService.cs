using Application.DTOs.Comments;
using Application.DTOs.Tickets;
using Application.Interfaces.Persistence;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Services;

namespace Application.Services;

public class TicketCommandService : ITicketCommandService
{
    private readonly ITicketRepository _tickets;
    private readonly ITicketDomainService _ticketDomainService;
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;

    public TicketCommandService(ITicketRepository tickets, ITicketDomainService ticketDomainService , IUserRepository users, IUnitOfWork uow)
    {
        _tickets = tickets;
        _ticketDomainService = ticketDomainService;
        _users = users;
        _uow = uow;
    }
    
    public async Task<Guid> CreateAsync(Guid currentUserId, CreateTicketDto dto, CancellationToken cancellationToken = default)
    {
        var author = await _users.GetByIdAsync(currentUserId, cancellationToken)
                     ?? throw new InvalidOperationException();

        var ticket = new Ticket(dto.Title, dto.Description, author);

        await _tickets.AddAsync(ticket, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return ticket.Id;
    }

    public async Task UpdateAsync(Guid ticketId, Guid currentUserId, UpdateTicketDto dto, CancellationToken cancellationToken = default)
    {
        var ticket = await _tickets.GetByIdAsync(ticketId, cancellationToken)
                     ?? throw new InvalidOperationException();

        _ticketDomainService.UpdateContent(ticket, currentUserId, dto.Title, dto.Description);

        await _uow.SaveChangesAsync(cancellationToken);
    }

    public async Task AddCommentAsync(Guid ticketId, Guid currentUserId, UserRole role, AddCommentDto dto, CancellationToken cancellationToken = default)
    {
        var ticket = await _tickets.GetByIdAsync(ticketId, cancellationToken)
                     ?? throw new InvalidOperationException();
        
        var author = await _users.GetByIdAsync(currentUserId, cancellationToken)
                     ?? throw new InvalidOperationException();
        
        _ticketDomainService.AddComment(ticket, author, dto.Content);

        await _uow.SaveChangesAsync(cancellationToken);
    }

    public async Task ChangeStatusAsync(Guid ticketId, Guid currentUserId, UserRole role, TicketStatus newStatus, CancellationToken cancellationToken = default)
    {
        var ticket = await _tickets.GetByIdAsync(ticketId, cancellationToken)
                     ?? throw new InvalidOperationException();

        _ticketDomainService.ChangeStatus(ticket, currentUserId, role, newStatus);

        await _uow.SaveChangesAsync(cancellationToken);
    }
}