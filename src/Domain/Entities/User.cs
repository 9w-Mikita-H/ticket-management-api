using Domain.Enums;

namespace Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    
    public string Login { get; set; } = null!;
    
    public string PasswordHash { get; set; } = null!;
    
    public UserRole Role { get; set; }

    public ICollection<Ticket> CreatedTickets { get; set; } = new List<Ticket>();
    
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    
    protected User() { }

    public User(string login, string passwordHash, UserRole role)
    {
        Id = Guid.NewGuid();
        
        Login = login ?? throw new ArgumentNullException(nameof(login));
        
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        
        Role = role;
    }
}