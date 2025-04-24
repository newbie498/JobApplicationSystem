using JobApplicationSystem.Core.Entities;

namespace JobApplicationSystem.Core.Interfaces;

public interface IJobApplicationRepository : IRepository<JobApplication>
{
    Task<JobApplication?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<JobApplication>> GetByJobPostIdAsync(int jobPostId);
    Task<IEnumerable<JobApplication>> GetByCandidateIdAsync(int candidateId);
    Task<IEnumerable<JobApplication>> GetByStatusAsync(string status);
    Task<bool> ExistsAsync(int candidateId, int jobPostId);
} 