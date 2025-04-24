using Microsoft.EntityFrameworkCore;
using JobApplicationSystem.Core.Entities;
using JobApplicationSystem.Core.Interfaces;
using JobApplicationSystem.Infrastructure.Data;

namespace JobApplicationSystem.Infrastructure.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly ApplicationDbContext _context;

    public CompanyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Company?> GetByIdAsync(int id)
    {
        return await _context.Companies.FindAsync(id);
    }

    public async Task<Company?> GetByIdWithJobPostsAsync(int id)
    {
        return await _context.Companies
            .Include(c => c.JobPosts)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Company>> GetAllAsync()
    {
        return await _context.Companies.ToListAsync();
    }

    public async Task<IEnumerable<Company>> GetAllWithJobPostsAsync()
    {
        return await _context.Companies
            .Include(c => c.JobPosts)
            .ToListAsync();
    }

    public async Task<Company> AddAsync(Company entity)
    {
        _context.Companies.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Company entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Company entity)
    {
        _context.Companies.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<Company?> GetByEmailAsync(string email)
    {
        return await _context.Companies
            .FirstOrDefaultAsync(c => c.Email == email);
    }
} 