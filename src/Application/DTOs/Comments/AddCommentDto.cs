namespace Application.DTOs.Comments;

public record AddCommentDto
{
    public string Content { get; init; } = null!;
}