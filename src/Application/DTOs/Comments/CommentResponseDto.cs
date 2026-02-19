namespace Application.DTOs.Comments;

public record CommentResponseDto
{
    public Guid Id { get; init; }
    
    public string Content { get; init; } = null!;
    
    public string AuthorLogin { get; init; } = null!;
    
    public DateTime CreatedAt { get; init; }
}