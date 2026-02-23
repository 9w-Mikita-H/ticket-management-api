using Api.Auth;
using Application.DTOs.Comments;
using Application.DTOs.Common;
using Application.DTOs.Tickets;
using Application.Interfaces.Services;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/tickets")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly ICurrentUser _currentUser;
    private readonly ITicketQueryService _ticketQueries;
    private readonly ITicketCommandService _ticketCommands;

    public TicketsController(ITicketQueryService ticketQueries, ITicketCommandService ticketCommands, ICurrentUser currentUser)
    {
        _ticketQueries = ticketQueries;
        _ticketCommands = ticketCommands;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<TicketSummaryDto>>> Get([FromQuery] TicketFilterDto filter, CancellationToken ct)
    {
        var result = await _ticketQueries.GetAsync(_currentUser.UserId, _currentUser.Role, filter, ct);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TicketDetailsDto>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _ticketQueries.GetByIdAsync(id, _currentUser.UserId, _currentUser.Role, ct);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateTicketDto dto, CancellationToken ct)
    {
        var id = await _ticketCommands.CreateAsync(_currentUser.UserId, dto, ct);

        return Ok(id);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateTicketDto dto, CancellationToken ct)
    {
        await _ticketCommands.UpdateAsync(id, _currentUser.UserId, dto, ct);

        return NoContent();
    }

    [HttpPost("{id:guid}/comments")]
    public async Task<IActionResult> AddComment(Guid id, AddCommentDto dto, CancellationToken ct)
    {
        await _ticketCommands.AddCommentAsync(id, _currentUser.UserId, _currentUser.Role, dto, ct);

        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromQuery] TicketStatus status, CancellationToken ct)
    {
        await _ticketCommands.ChangeStatusAsync(id, _currentUser.UserId, _currentUser.Role, status, ct);

        return NoContent();
    }
}