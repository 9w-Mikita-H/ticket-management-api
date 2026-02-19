namespace Application.DTOs.Tickets;

public record CreateTicketDto
{
    public string Title { get; init; } = null!;

    public string Description { get; init; } = null!;
}
