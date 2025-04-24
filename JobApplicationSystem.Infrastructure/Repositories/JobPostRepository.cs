using Microsoft.EntityFrameworkCore;
using JobApplicationSystem.Core.Entities;
using JobApplicationSystem.Core.Interfaces;
using JobApplicationSystem.Core.DTOs;
using JobApplicationSystem.Infrastructure.Data;

namespace JobApplicationSystem.Infrastructure.Repositories;

public class JobPostRepository : IJobPostRepository
{
    private readonly ApplicationDbContext _context;

    public JobPostRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<JobPost?> GetByIdAsync(int id)
    {
        return await _context.JobPosts.FindAsync(id);
    }

    public async Task<JobPost?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.JobPosts
            .Include(j => j.Company)
            .Include(j => j.Applications)
            .ThenInclude(a => a.Candidate)
            .FirstOrDefaultAsync(j => j.Id == id);
    }

    public async Task<IEnumerable<JobPost>> GetAllAsync()
    {
        return await _context.JobPosts.ToListAsync();
    }

    public async Task<IEnumerable<JobPost>> GetAllWithDetailsAsync()
    {
        return await _context.JobPosts
            .Include(j => j.Company)
            .Include(j => j.Applications)
            .ThenInclude(a => a.Candidate)
            .ToListAsync();
    }

    public async Task<JobPost> AddAsync(JobPost entity)
    {
        _context.JobPosts.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(JobPost entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(JobPost entity)
    {
        _context.JobPosts.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<JobPost>> SearchJobPostsAsync(JobPostSearchDto searchDto)
    {
        var query = _context.JobPosts
            .Include(j => j.Company)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchDto.Keyword))
        {
            query = query.Where(j => 
                j.Title.Contains(searchDto.Keyword) || 
                j.Description.Contains(searchDto.Keyword) ||
                j.Location.Contains(searchDto.Keyword));
        }

        if (!string.IsNullOrWhiteSpace(searchDto.Title))
            query = query.Where(j => j.Title.Contains(searchDto.Title));

        if (!string.IsNullOrWhiteSpace(searchDto.Location))
            query = query.Where(j => j.Location.Contains(searchDto.Location));

        if (!string.IsNullOrWhiteSpace(searchDto.CompanyName))
            query = query.Where(j => j.Company.Name.Contains(searchDto.CompanyName));

        if (searchDto.MinSalary.HasValue)
            query = query.Where(j => j.SalaryRangeStart >= searchDto.MinSalary.Value);

        if (searchDto.MaxSalary.HasValue)
            query = query.Where(j => j.SalaryRangeEnd <= searchDto.MaxSalary.Value);

        if (searchDto.FromDate.HasValue)
            query = query.Where(j => j.PostedAt >= searchDto.FromDate.Value);

        if (searchDto.ToDate.HasValue)
            query = query.Where(j => j.PostedAt <= searchDto.ToDate.Value);

        if (searchDto.IsActive.HasValue)
            query = query.Where(j => j.IsActive == searchDto.IsActive.Value);

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<JobPost>> GetByCompanyIdAsync(int companyId)
    {
        return await _context.JobPosts
            .Include(j => j.Applications)
            .Where(j => j.CompanyId == companyId)
            .ToListAsync();
    }

    public async Task<IEnumerable<JobApplication>> GetApplicationsForJobAsync(int jobPostId)
    {
        return await _context.JobApplications
            .Include(a => a.Candidate)
            .Where(a => a.JobPostId == jobPostId)
            .ToListAsync();
    }
} 