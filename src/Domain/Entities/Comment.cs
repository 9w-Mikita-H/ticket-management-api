namespace Domain.Entities;

public class Comment
{
    public Guid Id { get; private set; }
    
    public string Content { get; set; } = null!;
    
    public Ticket Ticket { get; private set; } = null!;
    public Guid TicketId { get; private set; }
    
    public User Author { get; private set; } = null!;
    public Guid AuthorId { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    protected Comment() { }

    public Comment(string content, Ticket ticket, User author)
    {
        Id = Guid.NewGuid();
        
        Content = content ?? throw new ArgumentNullException(nameof(content));
        
        Ticket = ticket ?? throw new ArgumentNullException(nameof(ticket));
        TicketId = Ticket.Id;
        
        Author = author ?? throw new ArgumentNullException(nameof(author));
        AuthorId = Author.Id;
        
        CreatedAt = DateTime.UtcNow;
    }
}