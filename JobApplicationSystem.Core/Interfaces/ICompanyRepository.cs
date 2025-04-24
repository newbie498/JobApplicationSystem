using JobApplicationSystem.Core.Entities;

namespace JobApplicationSystem.Core.Interfaces;

public interface ICompanyRepository : IRepository<Company>
{
    Task<Company?> GetByIdWithJobPostsAsync(int id);
    Task<IEnumerable<Company>> GetAllWithJobPostsAsync();
    Task<Company?> GetByEmailAsync(string email);
} 