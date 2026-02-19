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
    private readonly ITicketService _tickets;
    private readonly ICurrentUser _currentUser;

    public TicketsController(ITicketService tickets, ICurrentUser currentUser)
    {
        _tickets = tickets;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<TicketListItemDto>>> Get([FromQuery] TicketFilterDto filter, CancellationToken ct)
    {
        var result = await _tickets.GetAsync(
            _currentUser.UserId,
            _currentUser.Role,
            filter,
            ct);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TicketDetailsDto>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _tickets.GetByIdAsync(
            id,
            _currentUser.UserId,
            _currentUser.Role,
            ct);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateTicketDto dto, CancellationToken ct)
    {
        var id = await _tickets.CreateAsync(
            _currentUser.UserId,
            dto,
            ct);

        return Ok(id);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateTicketDto dto, CancellationToken ct)
    {
        await _tickets.UpdateAsync(
            id,
            _currentUser.UserId,
            dto,
            ct);

        return NoContent();
    }

    [HttpPost("{id:guid}/comments")]
    public async Task<IActionResult> AddComment(Guid id, AddCommentDto dto, CancellationToken ct)
    {
        await _tickets.AddCommentAsync(
            id,
            _currentUser.UserId,
            _currentUser.Role,
            dto,
            ct);

        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromQuery] TicketStatus status, CancellationToken ct)
    {
        await _tickets.ChangeStatusAsync(
            id,
            _currentUser.UserId,
            _currentUser.Role,
            status,
            ct);

        return NoContent();
    }
}