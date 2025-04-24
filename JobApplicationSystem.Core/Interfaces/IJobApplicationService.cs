using JobApplicationSystem.Core.DTOs;
using JobApplicationSystem.Core.Entities;

namespace JobApplicationSystem.Core.Interfaces;

public interface IJobApplicationService
{
    Task<JobApplication> SubmitApplicationAsync(CreateJobApplicationDto createJobApplicationDto);
    Task<JobApplication?> GetApplicationByIdAsync(int id);
    Task<JobApplicationDto> CreateJobApplicationAsync(int candidateId, CreateJobApplicationDto createJobApplicationDto);
    Task<JobApplicationDto?> GetJobApplicationByIdAsync(int id);
    Task<JobApplicationDto> UpdateJobApplicationStatusAsync(int id, UpdateJobApplicationStatusDto updateStatusDto);
    Task<bool> DeleteJobApplicationAsync(int id);
    Task<IEnumerable<JobApplicationDto>> GetApplicationsByJobPostAsync(int jobPostId);
    Task<IEnumerable<JobApplicationDto>> GetApplicationsByCandidateAsync(int candidateId);
} 