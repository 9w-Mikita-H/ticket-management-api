using Application.DTOs.Comments;
using Domain.Enums;

namespace Application.DTOs.Tickets;

public record TicketDetailsDto
{
    public Guid Id { get; init; }
    
    public string Title { get; init; } = null!;
    
    public string Description { get; init; } = null!;
    
    public TicketStatus Status { get; init; }

    public DateTime CreatedAt { get; init; }
    public DateTime LastActivityAt { get; init; }

    public string AuthorLogin { get; init; } = null!;

    public List<CommentResponseDto> Comments { get; init; } = [];
}