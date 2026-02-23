using Domain.Entities;
using Domain.Enums;

namespace Domain.Services;

public interface ITicketDomainService
{
    public void EnsureCanView(Ticket ticket, Guid currentUserId, UserRole role);

    public void UpdateContent(Ticket ticket, Guid currentUserId, string title, string description);

    public void ChangeStatus(Ticket ticket, Guid currentUserId, UserRole role, TicketStatus newStatus);

    public void AddComment(Ticket ticket, User author, string content);
}