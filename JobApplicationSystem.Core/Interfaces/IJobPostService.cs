using JobApplicationSystem.Core.DTOs;

namespace JobApplicationSystem.Core.Interfaces;

public interface IJobPostService
{
    Task<JobPostDto> CreateJobPostAsync(int companyId, CreateJobPostDto createJobPostDto);
    Task<JobPostDto?> GetJobPostByIdAsync(int id);
    Task<IEnumerable<JobPostDto>> SearchJobPostsAsync(JobPostSearchDto searchDto);
    Task<JobPostDto> UpdateJobPostAsync(int id, UpdateJobPostDto updateJobPostDto);
    Task<bool> DeleteJobPostAsync(int id);
    Task<IEnumerable<JobPostDto>> GetJobPostsByCompanyAsync(int companyId);
    Task<IEnumerable<JobApplicationDto>> GetJobApplicationsAsync(int jobPostId);
} 