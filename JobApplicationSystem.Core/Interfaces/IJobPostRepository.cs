using JobApplicationSystem.Core.Entities;
using JobApplicationSystem.Core.DTOs;

namespace JobApplicationSystem.Core.Interfaces;

public interface IJobPostRepository : IRepository<JobPost>
{
    Task<JobPost?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<JobPost>> GetAllWithDetailsAsync();
    Task<IEnumerable<JobPost>> SearchJobPostsAsync(JobPostSearchDto searchDto);
    Task<IEnumerable<JobPost>> GetByCompanyIdAsync(int companyId);
    Task<IEnumerable<JobApplication>> GetApplicationsForJobAsync(int jobPostId);
} 