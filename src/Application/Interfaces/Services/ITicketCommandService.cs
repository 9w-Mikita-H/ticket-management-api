using Application.DTOs.Comments;
using Application.DTOs.Tickets;
using Domain.Enums;

namespace Application.Interfaces.Services;

public interface ITicketCommandService
{
    Task<Guid> CreateAsync(
        Guid currentUserId,
        CreateTicketDto dto,
        CancellationToken ct = default);

    Task UpdateAsync(
        Guid ticketId,
        Guid currentUserId,
        UpdateTicketDto dto,
        CancellationToken ct = default);
    
    Task ChangeStatusAsync(
        Guid ticketId,
        Guid currentUserId,
        UserRole currentUserRole,
        TicketStatus newStatus,
        CancellationToken ct = default);
    
    Task AddCommentAsync(
        Guid ticketId,
        Guid currentUserId,
        UserRole currentUserRole,
        AddCommentDto dto,
        CancellationToken ct = default);
}