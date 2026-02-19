namespace Application.DTOs.Auth;

public record RegisterRequestDto
{
    public string Login { get; init; } = null!;

    public string Password { get; init; } = null!;
}
