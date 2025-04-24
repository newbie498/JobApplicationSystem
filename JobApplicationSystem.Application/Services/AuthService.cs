using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using JobApplicationSystem.Core.DTOs;
using JobApplicationSystem.Core.Entities;
using JobApplicationSystem.Core.Interfaces;
using JobApplicationSystem.Core.Settings;

namespace JobApplicationSystem.Application.Services;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterCompanyAsync(RegisterCompanyRequest request);
    Task<AuthResponse> RegisterCandidateAsync(RegisterCandidateRequest request);
}

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly JwtSettings _jwtSettings;

    public AuthService(IUnitOfWork unitOfWork, JwtSettings jwtSettings)
    {
        _unitOfWork = unitOfWork;
        _jwtSettings = jwtSettings;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var company = await _unitOfWork.Companies.GetByEmailAsync(request.Email);
        if (company != null && VerifyPassword(request.Password, company.PasswordHash, company.PasswordSalt))
        {
            return GenerateAuthResponse(company.Id, company.Email, "Company");
        }

        var candidate = await _unitOfWork.Candidates.GetByEmailAsync(request.Email);
        if (candidate != null && VerifyPassword(request.Password, candidate.PasswordHash, candidate.PasswordSalt))
        {
            return GenerateAuthResponse(candidate.Id, candidate.Email, "Candidate");
        }

        throw new UnauthorizedAccessException("Invalid email or password");
    }

    public async Task<AuthResponse> RegisterCompanyAsync(RegisterCompanyRequest request)
    {
        var existingCompany = await _unitOfWork.Companies.GetByEmailAsync(request.Email);
        if (existingCompany != null)
        {
            throw new InvalidOperationException("Email already registered");
        }

        CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

        var company = new Company
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            Description = request.Description,
            Industry = request.Industry,
            Location = request.Location
        };

        await _unitOfWork.Companies.AddAsync(company);
        await _unitOfWork.SaveChangesAsync();

        return GenerateAuthResponse(company.Id, company.Email, "Company");
    }

    public async Task<AuthResponse> RegisterCandidateAsync(RegisterCandidateRequest request)
    {
        var existingCandidate = await _unitOfWork.Candidates.GetByEmailAsync(request.Email);
        if (existingCandidate != null)
        {
            throw new InvalidOperationException("Email already registered");
        }

        CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

        var candidate = new Candidate
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            Phone = request.Phone,
            ResumeUrl = request.ResumeUrl,
            Skills = request.Skills
        };

        await _unitOfWork.Candidates.AddAsync(candidate);
        await _unitOfWork.SaveChangesAsync();

        return GenerateAuthResponse(candidate.Id, candidate.Email, "Candidate");
    }

    private AuthResponse GenerateAuthResponse(int userId, string email, string role)
    {
        var token = GenerateJwtToken(userId, email, role);
        return new AuthResponse
        {
            Token = token,
            Role = role,
            Email = email,
            UserId = userId
        };
    }

    private string GenerateJwtToken(int userId, string email, string role)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    private static bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512(passwordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(passwordHash);
    }
} 