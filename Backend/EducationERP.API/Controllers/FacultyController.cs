using EducationERP.Application.Common.Models;
using EducationERP.Application.Features.Faculty.Commands.CreateFaculty;
using EducationERP.Application.Features.Faculty.DTOs;
using EducationERP.Application.Features.Faculty.Queries.GetFaculty;
using EducationERP.Application.Features.Faculty.Queries.GetFacultyById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EducationERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FacultyController : ControllerBase
{
    private readonly IMediator _mediator;

    public FacultyController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all faculty with pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<FacultyDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFaculty(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null)
    {
        var query = new GetFacultyQuery(pageNumber, pageSize, searchTerm);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Get faculty by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(FacultyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFacultyById(Guid id)
    {
        var query = new GetFacultyByIdQuery(id);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Create a new faculty member
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(FacultyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateFaculty([FromBody] CreateFacultyDto dto)
    {
        var command = new CreateFacultyCommand(dto);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.ErrorMessage });
        }

        return CreatedAtAction(nameof(GetFacultyById), new { id = result.Data!.Id }, result.Data);
    }
}
