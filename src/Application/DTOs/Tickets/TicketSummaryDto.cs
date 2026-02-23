using Domain.Enums;

namespace Application.DTOs.Tickets;

public record TicketSummaryDto
{
    public Guid Id { get; init; }
    
    public string Title { get; init; } = null!;
    
    public TicketStatus Status { get; init; }
    
    public DateTime CreatedAt { get; init; }
    
    public DateTime LastActivityAt { get; init; }
}