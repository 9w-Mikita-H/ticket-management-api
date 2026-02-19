using System.Security.Claims;
using Domain.Enums;

namespace Api.Auth;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User
        ?? throw new InvalidOperationException("No HttpContext.");

    public bool IsAuthenticated => User.Identity?.IsAuthenticated ?? false;

    public Guid UserId
    {
        get
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? User.FindFirstValue(ClaimTypes.Name)
                        ?? throw new UnauthorizedAccessException();

            return Guid.Parse(value);
        }
    }

    public UserRole Role
    {
        get
        {
            var value = User.FindFirstValue(ClaimTypes.Role)
                        ?? throw new UnauthorizedAccessException();

            return Enum.Parse<UserRole>(value);
        }
    }
}