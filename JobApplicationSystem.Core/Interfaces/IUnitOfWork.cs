namespace JobApplicationSystem.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ICompanyRepository Companies { get; }
    IJobPostRepository JobPosts { get; }
    ICandidateRepository Candidates { get; }
    IJobApplicationRepository JobApplications { get; }
    
    Task<int> SaveChangesAsync();
} 