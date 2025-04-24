using Microsoft.AspNetCore.Mvc;
using JobApplicationSystem.Core.DTOs;
using JobApplicationSystem.Application.Services;

namespace JobApplicationSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("register/company")]
    public async Task<ActionResult<AuthResponse>> RegisterCompany([FromBody] RegisterCompanyRequest request)
    {
        try
        {
            var response = await _authService.RegisterCompanyAsync(request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("register/candidate")]
    public async Task<ActionResult<AuthResponse>> RegisterCandidate([FromBody] RegisterCandidateRequest request)
    {
        try
        {
            var response = await _authService.RegisterCandidateAsync(request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
} 