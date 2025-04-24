using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using JobApplicationSystem.Core.DTOs;
using JobApplicationSystem.Core.Interfaces;

namespace JobApplicationSystem.Api.Controllers;

/// <summary>
/// Controller for managing job posts
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;

    public JobsController(IJobService jobService)
    {
        _jobService = jobService;
    }

    /// <summary>
    /// Gets all jobs with optional search criteria
    /// </summary>
    /// <param name="keyword">Search keyword</param>
    /// <param name="company">Company name filter</param>
    /// <param name="fromDate">Posted after date</param>
    /// <param name="toDate">Posted before date</param>
    /// <returns>List of matching job posts</returns>
    /// <response code="200">Returns the list of job posts</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<JobPostDto>>> GetJobs(
        [FromQuery] string? keyword = null,
        [FromQuery] string? company = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var jobs = await _jobService.SearchJobsAsync(keyword, company, fromDate, toDate);
        return Ok(jobs);
    }

    /// <summary>
    /// Gets a job post by ID
    /// </summary>
    /// <param name="id">The job post ID</param>
    /// <returns>The job post details</returns>
    /// <response code="200">Returns the job post details</response>
    /// <response code="404">If the job post is not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobPostDto>> GetJob(int id)
    {
        var jobPost = await _jobService.GetJobByIdAsync(id);
        if (jobPost == null)
        {
            return NotFound();
        }
        return Ok(jobPost);
    }

    /// <summary>
    /// Creates a new job post
    /// </summary>
    /// <param name="jobDto">The job post details</param>
    /// <returns>The created job post</returns>
    /// <response code="201">Returns the created job post</response>
    /// <response code="400">If the job post data is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized</response>
    [Authorize(Roles = "Company")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<JobPostDto>> CreateJob([FromBody] CreateJobPostDto jobDto)
    {
        var companyId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
        var job = await _jobService.CreateJobAsync(companyId, jobDto);
        return CreatedAtAction(nameof(GetJob), new { id = job.Id }, job);
    }

    /// <summary>
    /// Updates a job post
    /// </summary>
    /// <param name="id">The job post ID</param>
    /// <param name="jobDto">The updated job post details</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the update was successful</response>
    /// <response code="400">If the job post data is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized</response>
    /// <response code="404">If the job post is not found</response>
    [Authorize(Roles = "Company")]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateJob(int id, [FromBody] UpdateJobPostDto jobDto)
    {
        var companyId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
        try
        {
            var job = await _jobService.GetJobByIdAsync(id);
            if (job?.CompanyId != companyId)
            {
                return Forbid();
            }

            await _jobService.UpdateJobAsync(id, jobDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Deletes a job post
    /// </summary>
    /// <param name="id">The job post ID</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the deletion was successful</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized</response>
    /// <response code="404">If the job post is not found</response>
    [Authorize(Roles = "Company")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteJob(int id)
    {
        var companyId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
        try
        {
            var job = await _jobService.GetJobByIdAsync(id);
            if (job?.CompanyId != companyId)
            {
                return Forbid();
            }

            await _jobService.DeleteJobAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Gets all applications for a job post
    /// </summary>
    /// <param name="id">The job post ID</param>
    /// <returns>List of job applications</returns>
    /// <response code="200">Returns the list of applications</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized</response>
    /// <response code="404">If the job post is not found</response>
    [HttpGet("{id}/applications")]
    [Authorize(Roles = "Company")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<JobApplicationDto>>> GetJobApplications(int id)
    {
        var companyId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
        var job = await _jobService.GetJobByIdAsync(id);
        if (job?.CompanyId != companyId)
        {
            return Forbid();
        }

        try
        {
            var applications = await _jobService.GetJobApplicationsAsync(id);
            return Ok(applications);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
} 