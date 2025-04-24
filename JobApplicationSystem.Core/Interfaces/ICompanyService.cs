using JobApplicationSystem.Core.DTOs;
using JobApplicationSystem.Core.Entities;

namespace JobApplicationSystem.Core.Interfaces;

public interface ICompanyService
{
    Task<CompanyDto> CreateCompanyAsync(CreateCompanyDto companyDto);
    Task<CompanyDto?> GetCompanyByIdAsync(int id);
    Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync();
    Task<JobPostDto> CreateJobPostAsync(int companyId, CreateJobPostDto jobPostDto);
    Task UpdateCompanyAsync(int id, UpdateCompanyDto companyDto);
    Task DeleteCompanyAsync(int id);
} 