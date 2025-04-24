using JobApplicationSystem.Core.Interfaces;

namespace JobApplicationSystem.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private ICompanyRepository? _companyRepository;
    private IJobPostRepository? _jobPostRepository;
    private ICandidateRepository? _candidateRepository;
    private IJobApplicationRepository? _jobApplicationRepository;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public ICompanyRepository Companies => 
        _companyRepository ??= new Repositories.CompanyRepository(_context);

    public IJobPostRepository JobPosts =>
        _jobPostRepository ??= new Repositories.JobPostRepository(_context);

    public ICandidateRepository Candidates =>
        _candidateRepository ??= new Repositories.CandidateRepository(_context);

    public IJobApplicationRepository JobApplications =>
        _jobApplicationRepository ??= new Repositories.JobApplicationRepository(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
} 