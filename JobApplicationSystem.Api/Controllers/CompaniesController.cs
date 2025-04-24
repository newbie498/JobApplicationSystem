using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using JobApplicationSystem.Core.DTOs;
using JobApplicationSystem.Core.Interfaces;
using JobApplicationSystem.Core.Entities;

namespace JobApplicationSystem.Api.Controllers;

/// <summary>
/// Controller for managing companies
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CompaniesController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompaniesController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    /// <summary>
    /// Creates a new company
    /// </summary>
    /// <param name="createCompanyDto">The company data</param>
    /// <returns>The newly created company</returns>
    /// <response code="201">Returns the newly created company</response>
    /// <response code="400">If the company data is invalid</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Company>> CreateCompany([FromBody] CreateCompanyDto createCompanyDto)
    {
        var company = await _companyService.CreateCompanyAsync(createCompanyDto);
        return CreatedAtAction(nameof(GetCompany), new { id = company.Id }, company);
    }

    /// <summary>
    /// Gets a company by ID
    /// </summary>
    /// <param name="id">The company ID</param>
    /// <returns>The company details</returns>
    /// <response code="200">Returns the company details</response>
    /// <response code="404">If the company is not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Company>> GetCompany(int id)
    {
        var company = await _companyService.GetCompanyByIdAsync(id);
        if (company == null)
        {
            return NotFound();
        }

        return Ok(company);
    }

    /// <summary>
    /// Creates a new job post for a company
    /// </summary>
    /// <param name="id">The company ID</param>
    /// <param name="createJobPostDto">The job post data</param>
    /// <returns>The newly created job post</returns>
    /// <response code="201">Returns the newly created job post</response>
    /// <response code="404">If the company is not found</response>
    /// <response code="400">If the job post data is invalid</response>
    [HttpPost("{id}/jobs")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<JobPost>> CreateJobPost(int id, [FromBody] CreateJobPostDto createJobPostDto)
    {
        try
        {
            var jobPost = await _companyService.CreateJobPostAsync(id, createJobPostDto);
            return CreatedAtAction("GetJobPost", "Jobs", new { id = jobPost.Id }, jobPost);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Company not found");
        }
    }

    [Authorize(Roles = "Company")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCompany(int id, [FromBody] UpdateCompanyDto companyDto)
    {
        if (id != int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!))
        {
            return Forbid();
        }

        try
        {
            await _companyService.UpdateCompanyAsync(id, companyDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [Authorize(Roles = "Company")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCompany(int id)
    {
        if (id != int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!))
        {
            return Forbid();
        }

        try
        {
            await _companyService.DeleteCompanyAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
} 