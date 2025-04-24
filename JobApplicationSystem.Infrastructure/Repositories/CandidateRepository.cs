using Microsoft.EntityFrameworkCore;
using JobApplicationSystem.Core.Entities;
using JobApplicationSystem.Core.Interfaces;
using JobApplicationSystem.Infrastructure.Data;

namespace JobApplicationSystem.Infrastructure.Repositories;

public class CandidateRepository : ICandidateRepository
{
    private readonly ApplicationDbContext _context;

    public CandidateRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Candidate?> GetByIdAsync(int id)
    {
        return await _context.Candidates.FindAsync(id);
    }

    public async Task<Candidate?> GetByIdWithApplicationsAsync(int id)
    {
        return await _context.Candidates
            .Include(c => c.Applications)
            .ThenInclude(a => a.JobPost)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Candidate>> GetAllAsync()
    {
        return await _context.Candidates.ToListAsync();
    }

    public async Task<Candidate> AddAsync(Candidate entity)
    {
        _context.Candidates.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Candidate entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Candidate entity)
    {
        _context.Candidates.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<Candidate?> GetByEmailAsync(string email)
    {
        return await _context.Candidates
            .FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task<IEnumerable<JobApplication>> GetApplicationsAsync(int candidateId)
    {
        return await _context.JobApplications
            .Include(a => a.JobPost)
            .Where(a => a.CandidateId == candidateId)
            .ToListAsync();
    }
} 