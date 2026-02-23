using Application.Enums;
using Domain.Enums;

namespace Application.DTOs.Tickets;

public class TicketFilterDto
{
    public int Page { get; init; }
    public int PageSize { get; init; }

    public TicketStatus? Status { get; init; }

    public string? SearchQuery { get; init; }

    public TicketSortBy SortBy { get; init; } = TicketSortBy.CreatedAt;
    public bool IsDescending { get; init; }
}