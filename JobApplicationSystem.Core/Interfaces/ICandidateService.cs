using JobApplicationSystem.Core.DTOs;
using JobApplicationSystem.Core.Entities;

namespace JobApplicationSystem.Core.Interfaces;

public interface ICandidateService
{
    Task<Candidate> RegisterCandidateAsync(CreateCandidateDto createCandidateDto);
    Task<Candidate?> GetCandidateByIdAsync(int id);
    Task<CandidateDto?> GetCandidateByEmailAsync(string email);
    Task<CandidateDto> UpdateCandidateAsync(int id, UpdateCandidateDto updateCandidateDto);
    Task<bool> DeleteCandidateAsync(int id);
    Task<IEnumerable<JobApplicationDto>> GetCandidateApplicationsAsync(int candidateId);
} 