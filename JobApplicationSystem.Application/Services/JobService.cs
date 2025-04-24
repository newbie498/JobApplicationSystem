using AutoMapper;
using JobApplicationSystem.Core.DTOs;
using JobApplicationSystem.Core.Entities;
using JobApplicationSystem.Core.Interfaces;

namespace JobApplicationSystem.Application.Services;

public class JobService : IJobService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public JobService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<JobPostDto> CreateJobAsync(int companyId, CreateJobPostDto createJobDto)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(companyId);
        if (company == null)
        {
            throw new KeyNotFoundException("Company not found");
        }

        var job = _mapper.Map<JobPost>(createJobDto);
        job.CompanyId = companyId;
        job.PostedAt = DateTime.UtcNow;
        job.IsActive = true;

        await _unitOfWork.JobPosts.AddAsync(job);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<JobPostDto>(job);
    }

    public async Task<JobPostDto?> GetJobByIdAsync(int id)
    {
        var job = await _unitOfWork.JobPosts.GetByIdWithDetailsAsync(id);
        return job != null ? _mapper.Map<JobPostDto>(job) : null;
    }

    public async Task<IEnumerable<JobPostDto>> SearchJobsAsync(string? keyword = null, string? company = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var searchDto = new JobPostSearchDto
        {
            Keyword = keyword,
            CompanyName = company,
            FromDate = fromDate,
            ToDate = toDate
        };

        var jobs = await _unitOfWork.JobPosts.SearchJobPostsAsync(searchDto);
        return _mapper.Map<IEnumerable<JobPostDto>>(jobs);
    }

    public async Task<JobPostDto> UpdateJobAsync(int id, UpdateJobPostDto updateJobDto)
    {
        var job = await _unitOfWork.JobPosts.GetByIdAsync(id);
        if (job == null)
        {
            throw new KeyNotFoundException($"Job with ID {id} not found");
        }

        _mapper.Map(updateJobDto, job);
        await _unitOfWork.JobPosts.UpdateAsync(job);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<JobPostDto>(job);
    }

    public async Task DeleteJobAsync(int id)
    {
        var job = await _unitOfWork.JobPosts.GetByIdAsync(id);
        if (job == null)
        {
            throw new KeyNotFoundException($"Job with ID {id} not found");
        }

        await _unitOfWork.JobPosts.DeleteAsync(job);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<JobApplicationDto>> GetJobApplicationsAsync(int jobId)
    {
        var applications = await _unitOfWork.JobPosts.GetApplicationsForJobAsync(jobId);
        return _mapper.Map<IEnumerable<JobApplicationDto>>(applications);
    }
} 