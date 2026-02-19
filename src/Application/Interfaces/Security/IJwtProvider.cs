using Application.DTOs.Auth;
using Domain.Entities;

namespace Application.Interfaces.Security;

public interface IJwtProvider
{
    JwtTokenResult Generate(User user);
}