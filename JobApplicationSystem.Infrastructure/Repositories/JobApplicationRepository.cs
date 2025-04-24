using Microsoft.EntityFrameworkCore;
using JobApplicationSystem.Core.Entities;
using JobApplicationSystem.Core.Interfaces;
using JobApplicationSystem.Infrastructure.Data;
using System;

namespace JobApplicationSystem.Infrastructure.Repositories;

public class JobApplicationRepository : IJobApplicationRepository
{
    private readonly ApplicationDbContext _context;

    public JobApplicationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<JobApplication?> GetByIdAsync(int id)
    {
        return await _context.JobApplications.FindAsync(id);
    }

    public async Task<JobApplication?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.JobApplications
            .Include(a => a.Candidate)
            .Include(a => a.JobPost)
                .ThenInclude(j => j.Company)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<JobApplication>> GetAllAsync()
    {
        return await _context.JobApplications.ToListAsync();
    }

    public async Task<JobApplication> AddAsync(JobApplication entity)
    {
        _context.JobApplications.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(JobApplication entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(JobApplication entity)
    {
        _context.JobApplications.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<JobApplication>> GetByCandidateIdAsync(int candidateId)
    {
        return await _context.JobApplications
            .Include(a => a.JobPost)
                .ThenInclude(j => j.Company)
            .Where(a => a.CandidateId == candidateId)
            .ToListAsync();
    }

    public async Task<IEnumerable<JobApplication>> GetByJobPostIdAsync(int jobPostId)
    {
        return await _context.JobApplications
            .Include(a => a.Candidate)
            .Where(a => a.JobPostId == jobPostId)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(int candidateId, int jobPostId)
    {
        return await _context.JobApplications
            .AnyAsync(a => a.CandidateId == candidateId && a.JobPostId == jobPostId);
    }

    public async Task<IEnumerable<JobApplication>> GetByStatusAsync(string status)
    {
        if (!Enum.TryParse<ApplicationStatus>(status, true, out var applicationStatus))
        {
            throw new ArgumentException($"Invalid application status: {status}", nameof(status));
        }

        return await _context.JobApplications
            .Include(a => a.Candidate)
            .Include(a => a.JobPost)
                .ThenInclude(j => j.Company)
            .Where(a => a.Status.ToString() == applicationStatus.ToString())
            .ToListAsync();
    }
} 