using Microsoft.AspNetCore.Mvc;
using JobApplicationSystem.Core.DTOs;
using JobApplicationSystem.Core.Interfaces;
using JobApplicationSystem.Core.Entities;

namespace JobApplicationSystem.Api.Controllers;

/// <summary>
/// Controller for managing candidates
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CandidatesController : ControllerBase
{
    private readonly ICandidateService _candidateService;

    public CandidatesController(ICandidateService candidateService)
    {
        _candidateService = candidateService;
    }

    /// <summary>
    /// Registers a new candidate
    /// </summary>
    /// <param name="createCandidateDto">The candidate registration data</param>
    /// <returns>The newly registered candidate</returns>
    /// <response code="201">Returns the newly registered candidate</response>
    /// <response code="400">If the candidate data is invalid or email already exists</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Candidate>> RegisterCandidate([FromBody] CreateCandidateDto createCandidateDto)
    {
        try
        {
            var candidate = await _candidateService.RegisterCandidateAsync(createCandidateDto);
            return CreatedAtAction(nameof(GetCandidate), new { id = candidate.Id }, candidate);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Gets a candidate by ID
    /// </summary>
    /// <param name="id">The candidate ID</param>
    /// <returns>The candidate details</returns>
    /// <response code="200">Returns the candidate details</response>
    /// <response code="404">If the candidate is not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Candidate>> GetCandidate(int id)
    {
        var candidate = await _candidateService.GetCandidateByIdAsync(id);
        if (candidate == null)
        {
            return NotFound();
        }

        return Ok(candidate);
    }
}