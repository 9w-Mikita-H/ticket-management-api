namespace Application.DTOs.Tickets;

public record UpdateTicketDto
{
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
}