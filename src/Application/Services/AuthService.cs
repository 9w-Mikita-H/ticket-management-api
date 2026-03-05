using Application.Interfaces.Persistence;
using Application.Interfaces.Services;
using Application.DTOs.Auth;
using Application.Interfaces.Security;
using Application.Mappings;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;

    public AuthService(IUserRepository users, IUnitOfWork uow, IPasswordHasher passwordHasher, IJwtProvider jwtProvider)
    {
        _users = users;
        _uow = uow;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto, CancellationToken ct = default)
    {
        if (await _users.ExistsByLoginAsync(dto.Login, ct))
            throw new InvalidOperationException("Login already exists.");

        var hash = _passwordHasher.Hash(dto.Password);

        var user = new User(dto.Login, hash, UserRole.User);

        await _users.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        var token = _jwtProvider.Generate(user);

        return token.ToAuthResponseDto();
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto, CancellationToken ct = default)
    {
        var user = await _users.GetByLoginAsync(dto.Login, ct, asNoTracking: true)
                   ?? throw new UnauthorizedAccessException();

        var valid = _passwordHasher.Verify(dto.Password, user.PasswordHash);

        if (!valid)
            throw new UnauthorizedAccessException();

        var token = _jwtProvider.Generate(user);

        return token.ToAuthResponseDto();
    }
}