using Application.DTOs.Comments;
using Application.DTOs.Common;
using Application.DTOs.Tickets;
using Domain.Enums;

namespace Application.Interfaces.Services;

public interface ITicketService
{
    Task<PagedResult<TicketListItemDto>> GetAsync(
        Guid currentUserId,
        UserRole currentUserRole,
        TicketFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<TicketDetailsDto?> GetByIdAsync(
        Guid ticketId,
        Guid currentUserId,
        UserRole currentUserRole,
        CancellationToken cancellationToken = default);

    Task<Guid> CreateAsync(
        Guid currentUserId,
        CreateTicketDto dto,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Guid ticketId,
        Guid currentUserId,
        UpdateTicketDto dto,
        CancellationToken cancellationToken = default);

    Task ChangeStatusAsync(
        Guid ticketId,
        Guid currentUserId,
        UserRole currentUserRole,
        TicketStatus newStatus,
        CancellationToken cancellationToken = default);
    
    Task AddCommentAsync(
        Guid ticketId,
        Guid currentUserId,
        UserRole currentUserRole,
        AddCommentDto dto,
        CancellationToken cancellationToken = default);
}