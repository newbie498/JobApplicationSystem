using AutoMapper;
using JobApplicationSystem.Core.DTOs;
using JobApplicationSystem.Core.Entities;
using JobApplicationSystem.Core.Interfaces;

namespace JobApplicationSystem.Application.Services;

public class CompanyService : ICompanyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CompanyService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CompanyDto> CreateCompanyAsync(CreateCompanyDto createCompanyDto)
    {
        var company = new Company
        {
            Name = createCompanyDto.Name,
            Description = createCompanyDto.Description,
            Location = createCompanyDto.Location,
            Email = createCompanyDto.Email,
            Phone = createCompanyDto.Phone,
            Website = createCompanyDto.Website
        };

        await _unitOfWork.Companies.AddAsync(company);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CompanyDto>(company);
    }

    public async Task<CompanyDto?> GetCompanyByIdAsync(int id)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(id);
        return company != null ? _mapper.Map<CompanyDto>(company) : null;
    }

    public async Task<JobPostDto> CreateJobPostAsync(int companyId, CreateJobPostDto createJobPostDto)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(companyId);
        if (company == null)
        {
            throw new KeyNotFoundException("Company not found");
        }

        var jobPost = new JobPost
        {
            CompanyId = companyId,
            Title = createJobPostDto.Title,
            Description = createJobPostDto.Description,
            Location = createJobPostDto.Location,
            JobType = createJobPostDto.JobType,
            SalaryRangeStart = createJobPostDto.SalaryRangeStart,
            SalaryRangeEnd = createJobPostDto.SalaryRangeEnd,
            PostedAt = DateTime.UtcNow,
            ExpiresAt = createJobPostDto.ExpiresAt,
            IsActive = true
        };

        await _unitOfWork.JobPosts.AddAsync(jobPost);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<JobPostDto>(jobPost);
    }

    public async Task<CompanyDto?> GetCompanyByIdWithJobPostsAsync(int id)
    {
        var company = await _unitOfWork.Companies.GetByIdWithJobPostsAsync(id);
        return company != null ? _mapper.Map<CompanyDto>(company) : null;
    }

    public async Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync()
    {
        var companies = await _unitOfWork.Companies.GetAllWithJobPostsAsync();
        return _mapper.Map<IEnumerable<CompanyDto>>(companies);
    }

    public async Task UpdateCompanyAsync(int id, UpdateCompanyDto updateCompanyDto)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(id);
        if (company == null)
            throw new KeyNotFoundException($"Company with ID {id} not found.");

        _mapper.Map(updateCompanyDto, company);
        await _unitOfWork.Companies.UpdateAsync(company);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteCompanyAsync(int id)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(id);
        if (company == null)
            throw new KeyNotFoundException($"Company with ID {id} not found.");

        await _unitOfWork.Companies.DeleteAsync(company);
        await _unitOfWork.SaveChangesAsync();
    }
}