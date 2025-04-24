using JobApplicationSystem.Core.DTOs;
using JobApplicationSystem.Core.Entities;

namespace JobApplicationSystem.Core.Interfaces;

public interface IJobService
{
    Task<IEnumerable<JobPostDto>> SearchJobsAsync(string? keyword = null, string? company = null, DateTime? fromDate = null, DateTime? toDate = null);
    Task<JobPostDto?> GetJobByIdAsync(int id);
    Task<JobPostDto> CreateJobAsync(int companyId, CreateJobPostDto jobDto);
    Task<JobPostDto> UpdateJobAsync(int id, UpdateJobPostDto jobDto);
    Task DeleteJobAsync(int id);
    Task<IEnumerable<JobApplicationDto>> GetJobApplicationsAsync(int jobId);
} 