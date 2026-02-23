using Domain.Entities;
using Domain.Enums;

namespace Domain.Services;

public class TicketDomainService : ITicketDomainService
{
    public void UpdateContent(Ticket ticket, Guid currentUserId, string title, string description)
    {
        if (ticket.AuthorId != currentUserId)
            throw new UnauthorizedAccessException();

        if (ticket.Status != TicketStatus.Open)
            throw new InvalidOperationException();

        if (ticket.Comments.Any(c => c.Author.Role == UserRole.Agent))
            throw new InvalidOperationException();

        ticket.Title = title;
        ticket.Description = description;
        ticket.LastActivityAt = DateTime.UtcNow;
    }
    
    public void ChangeStatus(Ticket ticket, Guid currentUserId, UserRole role, TicketStatus newStatus)
    {
        switch (role)
        {
            case UserRole.User:
                if (ticket.AuthorId != currentUserId || newStatus != TicketStatus.Closed)
                    throw new UnauthorizedAccessException();

                if (ticket.Status is not (TicketStatus.Open or TicketStatus.Resolved))
                    throw new InvalidOperationException();
                break;
            
            case UserRole.Agent:
                var allowed = (ticket.Status, newStatus) switch
                {
                    (TicketStatus.Open, TicketStatus.InProgress) => true,
                    (TicketStatus.InProgress, TicketStatus.Resolved) => true,
                    (TicketStatus.InProgress, TicketStatus.Open) => true,
                    (TicketStatus.Resolved, TicketStatus.InProgress) => true,
                    (TicketStatus.Resolved, TicketStatus.Closed) => true,
                    _ => false
                };

                if (!allowed)
                    throw new InvalidOperationException();
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(role));
        }

        ticket.Status = newStatus;
        ticket.LastActivityAt = DateTime.UtcNow;
    }

    public void AddComment(Ticket ticket, User author, string content)
    {
        if (ticket.Status == TicketStatus.Closed)
            throw new InvalidOperationException();

        if (author.Role == UserRole.User && ticket.AuthorId != author.Id)
            throw new UnauthorizedAccessException();
        
        ticket.Comments.Add(new Comment(content, ticket, author));
        ticket.LastActivityAt = DateTime.UtcNow;
    }
}