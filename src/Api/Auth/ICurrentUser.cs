using Domain.Enums;

namespace Api.Auth;

public interface ICurrentUser
{
    Guid UserId { get; }

    UserRole Role { get; }

    bool IsAuthenticated { get; }
}