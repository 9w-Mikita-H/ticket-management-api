using Domain.Enums;

namespace Application.DTOs.Tickets;

public class TicketFilterDto
{
    public int Page { get; init; }
    public int PageSize { get; init; }

    public TicketStatus? Status { get; init; }

    public string? SearchQuery { get; init; }

    public string? SortBy { get; init; }
    public string? SortDirection { get; init; }
}