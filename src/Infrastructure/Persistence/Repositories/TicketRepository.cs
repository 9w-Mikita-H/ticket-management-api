using Application.Interfaces.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly AppDbContext _dbContext;

    public TicketRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IQueryable<Ticket> Query()
    {
        return _dbContext.Tickets
            .AsQueryable();
    }

    public async Task<Ticket?> GetByIdAsync(Guid ticketId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Tickets
            .Include(t => t.Author)
            .Include(t => t.Comments)
            .ThenInclude(c => c.Author)
            .FirstOrDefaultAsync(t => t.Id == ticketId, cancellationToken);
    }

    public async Task AddAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        await _dbContext.Tickets.AddAsync(ticket, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid ticketId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Tickets
            .AnyAsync(t => t.Id == ticketId, cancellationToken);
    }
}