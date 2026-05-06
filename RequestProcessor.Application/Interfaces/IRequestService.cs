using RequestProcessor.Application.Common;
using RequestProcessor.Application.Dtos;
using RequestProcessor.Domain.Entities;

namespace RequestProcessor.Application.Interfaces;

public interface IRequestService
{
    Task<Result<ProcessingRequest>> CreateAsync(CreateRequestDto dto);
    Task<Result<ProcessingRequest>> GetByIdAsync(Guid id);
    Task<Result<IEnumerable<ProcessingRequest>>> GetAllAsync();
}
