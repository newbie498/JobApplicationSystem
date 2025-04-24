using JobApplicationSystem.Core.Entities;

namespace JobApplicationSystem.Core.Interfaces;

public interface ICandidateRepository : IRepository<Candidate>
{
    Task<Candidate?> GetByIdWithApplicationsAsync(int id);
    Task<Candidate?> GetByEmailAsync(string email);
    Task<IEnumerable<JobApplication>> GetApplicationsAsync(int candidateId);
} 