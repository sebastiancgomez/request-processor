using Microsoft.EntityFrameworkCore;
using RequestProcessor.Domain.Entities;
using RequestProcessor.Domain.Repositories;
using RequestProcessor.Infrastructure.Context;

namespace RequestProcessor.Infrastructure.Repositories;

public class PostgresRequestRepository : IRequestRepository
{
    private readonly ApplicationDbContext _context;

    public PostgresRequestRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ProcessingRequest request)
    {
        await _context.Requests.AddAsync(request);
        await _context.SaveChangesAsync();
    }

    public async Task<ProcessingRequest?> GetByIdAsync(Guid id)
    {
        return await _context.Requests.FindAsync(id);
    }

    public async Task<IEnumerable<ProcessingRequest>> GetAllAsync()
    {
        return await _context.Requests.AsNoTracking().ToListAsync();
    }
    public async Task UpdateAsync()
    {
        await _context.SaveChangesAsync();
    }
}
