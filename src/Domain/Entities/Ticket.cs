using Domain.Enums;

namespace Domain.Entities;

public class Ticket
{
    public Guid Id { get; private set; }
    
    public string Title { get; set; } = null!;
    
    public string Description { get; set; } = null!;
    
    public TicketStatus Status { get; set; }
    
    public User Author { get; private set; } = null!;
    public Guid AuthorId { get; private set; }
    
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    
    public DateTime CreatedAt { get; private set; }
    
    public DateTime LastActivityAt { get; set; }
    
    protected Ticket() { }

    public Ticket(string title, string description, User author)
    {
        Id = Guid.NewGuid();
        
        Title = title ?? throw new ArgumentNullException(nameof(title));
        
        Description = description ?? throw new ArgumentNullException(nameof(description));
        
        Status = TicketStatus.Open;
        
        Author = author ?? throw new ArgumentNullException(nameof(author));
        AuthorId = Author.Id;
        
        CreatedAt = DateTime.UtcNow;
        
        LastActivityAt = DateTime.UtcNow;
    }
}