using RequestProcessor.Domain.Entities;

namespace RequestProcessor.Domain.Repositories;

public interface IRequestRepository
{
    Task AddAsync(ProcessingRequest request);
    Task<ProcessingRequest?> GetByIdAsync(Guid id);
    Task<IEnumerable<ProcessingRequest>> GetAllAsync();
    Task UpdateAsync();
}