namespace Application.DTOs.Auth;

public record JwtTokenResult
{
    public string Token { get; init; } = null!;

    public DateTime ExpiresAt { get; init; }
}
