using Application.DTOs.Auth;
using Application.DTOs.Comments;
using Application.DTOs.Tickets;
using Domain.Entities;

namespace Application.Mappings;

public static class TicketMappings
{
    public static AuthResponseDto ToAuthResponseDto(this JwtTokenResult token)
    {
        return new AuthResponseDto
        {
            AccessToken = token.Token,
            ExpiresAt = token.ExpiresAt
        };
    }

    public static TicketDetailsDto ToDetailsDto(this Ticket ticket)
    {
        return new TicketDetailsDto
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            Status = ticket.Status,
            CreatedAt = ticket.CreatedAt,
            LastActivityAt = ticket.LastActivityAt,
            AuthorLogin = ticket.Author.Login,
            Comments = ticket.Comments.OrderBy(c => c.CreatedAt).Select(c => c.ToCommentResponseDto()).ToList()
        };
    }

    public static CommentResponseDto ToCommentResponseDto(this Comment comment)
    {
        return new CommentResponseDto
        {
            Id = comment.Id,
            Content = comment.Content,
            AuthorLogin = comment.Author.Login,
            CreatedAt = comment.CreatedAt
        };
    }

    public static TicketSummaryDto ToSummaryDto(this Ticket ticket)
    {
        return new TicketSummaryDto
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Status = ticket.Status,
            CreatedAt = ticket.CreatedAt,
            LastActivityAt = ticket.LastActivityAt
        };
    }
}