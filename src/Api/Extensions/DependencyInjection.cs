using System.Security.Claims;
using System.Text;
using Api.Auth;
using Application.Interfaces.Persistence;
using Application.Interfaces.Security;
using Application.Interfaces.Services;
using Application.Services;
using Domain.Services;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace Api.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();

        var jwtSection = configuration.GetSection("Jwt");
        var key = jwtSection["Key"]!;

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtSection["Issuer"],
                        ValidAudience = jwtSection["Audience"],

                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(key)),

                        NameClaimType = ClaimTypes.NameIdentifier,
                        RoleClaimType = ClaimTypes.Role
                    };
            });

        services.AddAuthorization();
        
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen();

        return services;
    }
    
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
        });
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtProvider, JwtProvider>();

        return services;
    }
    
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        
        services.AddScoped<ITicketQueryService, TicketQueryService>();
        services.AddScoped<ITicketCommandService, TicketCommandService>();

        return services;
    }

    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddScoped<ITicketDomainService, TicketDomainService>();

        return services;
    }
}