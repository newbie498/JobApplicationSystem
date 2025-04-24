using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using JobApplicationSystem.Core.DTOs;
using JobApplicationSystem.Core.Interfaces;

namespace JobApplicationSystem.Api.Controllers;

/// <summary>
/// Controller for managing job applications
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ApplicationsController : ControllerBase
{
    private readonly IJobApplicationService _applicationService;

    public ApplicationsController(IJobApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    /// <summary>
    /// Submits a new job application
    /// </summary>
    /// <param name="applicationDto">The job application data</param>
    /// <returns>The newly created job application</returns>
    /// <response code="201">Returns the newly created job application</response>
    /// <response code="400">If the application data is invalid or already exists</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized</response>
    /// <response code="404">If the job post or candidate is not found</response>
    [Authorize(Roles = "Candidate")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobApplicationDto>> SubmitApplication([FromBody] CreateJobApplicationDto applicationDto)
    {
        var candidateId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
        try
        {
            var application = await _applicationService.CreateJobApplicationAsync(candidateId, applicationDto);
            return CreatedAtAction(nameof(GetApplication), new { id = application.Id }, application);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Gets a job application by ID
    /// </summary>
    /// <param name="id">The job application ID</param>
    /// <returns>The job application details</returns>
    /// <response code="200">Returns the job application details</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized</response>
    /// <response code="404">If the job application is not found</response>
    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobApplicationDto>> GetApplication(int id)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        var application = await _applicationService.GetJobApplicationByIdAsync(id);
        if (application == null)
        {
            return NotFound();
        }

        // Verify access rights
        if (role == "Company" && application.JobPost.CompanyId != userId)
        {
            return Forbid();
        }
        if (role == "Candidate" && application.CandidateId != userId)
        {
            return Forbid();
        }

        return Ok(application);
    }

    /// <summary>
    /// Updates the status of a job application
    /// </summary>
    /// <param name="id">The job application ID</param>
    /// <param name="updateStatusDto">The new status</param>
    /// <returns>The updated job application</returns>
    /// <response code="200">Returns the updated job application</response>
    /// <response code="400">If the status update is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized</response>
    /// <response code="404">If the job application is not found</response>
    [Authorize(Roles = "Company")]
    [HttpPut("{id}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobApplicationDto>> UpdateApplicationStatus(
        int id,
        [FromBody] UpdateJobApplicationStatusDto updateStatusDto)
    {
        var companyId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);

        var application = await _applicationService.GetJobApplicationByIdAsync(id);
        if (application?.JobPost.CompanyId != companyId)
        {
            return Forbid();
        }

        try
        {
            var updatedApplication = await _applicationService.UpdateJobApplicationStatusAsync(id, updateStatusDto);
            return Ok(updatedApplication);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Withdraws a job application
    /// </summary>
    /// <param name="id">The job application ID</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the withdrawal was successful</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized</response>
    /// <response code="404">If the job application is not found</response>
    [Authorize(Roles = "Candidate")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> WithdrawApplication(int id)
    {
        var candidateId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);

        var application = await _applicationService.GetJobApplicationByIdAsync(id);
        if (application?.CandidateId != candidateId)
        {
            return Forbid();
        }

        var success = await _applicationService.DeleteJobApplicationAsync(id);
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
} 