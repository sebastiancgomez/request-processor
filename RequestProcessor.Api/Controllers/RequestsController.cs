

using Microsoft.AspNetCore.Mvc;
using RequestProcessor.Application.Dtos;
using RequestProcessor.Application.Interfaces;
using RequestProcessor.Application.Services;
using RequestProcessor.Domain.Entities;
using RequestProcessor.Domain.Repositories;

namespace RequestProcessor.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RequestsController : ControllerBase
{
    private readonly IRequestService _requestService;
    private readonly IMessagePublisher _messagePublisher;

    public RequestsController(IRequestService service, IMessagePublisher messagePublisher)
    {
        _requestService = service;
        _messagePublisher = messagePublisher;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateRequestDto dto)
    {
        var result = await _requestService.CreateAsync(dto);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return CreatedAtAction(nameof(GetRequestById), new { id = result.Value?.Id }, result.Value);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRequestById(Guid id)
    {
        var result = await _requestService.GetByIdAsync(id);

        if (!result.IsSuccess)
        {
            return NotFound(new { message = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _requestService.GetAllAsync();
        return Ok(result.Value);
    }
}
