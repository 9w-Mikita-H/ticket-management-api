namespace Application.DTOs.Auth;

public record AuthResponseDto
{
    public string AccessToken { get; init; } = null!;
    public DateTime ExpiresAt { get; init; }
}