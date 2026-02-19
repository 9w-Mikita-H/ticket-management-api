namespace Application.DTOs.Auth;

public record LoginRequestDto
{
    public string Login { get; init; } = null!;
    public string Password { get; init; } = null!;
}