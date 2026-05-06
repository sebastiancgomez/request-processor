using Microsoft.Extensions.Logging;
using RequestProcessor.Application.Common;
using RequestProcessor.Application.Dtos;
using RequestProcessor.Application.Interfaces;
using RequestProcessor.Domain.Entities;
using RequestProcessor.Domain.Exceptions;
using RequestProcessor.Domain.Repositories;

namespace RequestProcessor.Application.Services;

public class RequestService : IRequestService
{
    private readonly IRequestRepository _repository;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<RequestService> _logger;

    public RequestService(IRequestRepository repository, IMessagePublisher publisher, ILogger<RequestService> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Result<ProcessingRequest>> CreateAsync(CreateRequestDto dto)
    {
        _logger.LogInformation("Starting request creation: {RequestName}", dto.Name);
        try
        {
            var request = new ProcessingRequest
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Payload = dto.Payload,
                Status = RequestStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(request);
            _logger.LogDebug("Request persisted with ID: {RequestId}", request.Id);

            await _publisher.PublishRequestCreatedEventAsync(request.Id, request.CreatedAt);
            _logger.LogDebug("Request ID: {RequestId} published successfully ", request.Id);

            request.ChangeToProcessed();
            
            await _repository.UpdateAsync();
            _logger.LogInformation("Request {RequestName} cretated successfully with ID: {RequestID}", request.Name, request.Id);

            return Result<ProcessingRequest>.Success(request);
        }
        catch (BusinessException ex)
        {
            return Result<ProcessingRequest>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal error processing request with requestName {RequestName}", dto.Name);
            return Result<ProcessingRequest>.Failure("Internal error processing the request");
        }
    }
    public async Task<Result<ProcessingRequest>> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Starting GetByIdAsync for ID: {RequestID}", id);
        try
        {
            var request = await _repository.GetByIdAsync(id);

            if (request == null)
            {
                return Result<ProcessingRequest>.Failure($"Request with ID {id} not found.");
            }

            return Result<ProcessingRequest>.Success(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal error retrieving request with ID {RequestID}", id);
            return Result<ProcessingRequest>.Failure("Internal error retrieving the request");
        }
    }

    public async Task<Result<IEnumerable<ProcessingRequest>>> GetAllAsync()
    {
        _logger.LogInformation("Starting GetAllAsync");
        try
        {

            var requests = await _repository.GetAllAsync();
            return Result<IEnumerable<ProcessingRequest>>.Success(requests);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal error retrieving all requests");
            return Result<IEnumerable<ProcessingRequest>>.Failure("Fatal error retrieving all requests");
        }
    }
}