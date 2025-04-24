using Microsoft.EntityFrameworkCore;
using JobApplicationSystem.Core.Entities;

namespace JobApplicationSystem.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Company> Companies { get; set; } = null!;
    public DbSet<JobPost> JobPosts { get; set; } = null!;
    public DbSet<Candidate> Candidates { get; set; } = null!;
    public DbSet<JobApplication> JobApplications { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Company
        modelBuilder.Entity<Company>()
            .HasMany(c => c.JobPosts)
            .WithOne(j => j.Company)
            .HasForeignKey(j => j.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        // JobPost
        modelBuilder.Entity<JobPost>()
            .HasMany(j => j.Applications)
            .WithOne(a => a.JobPost)
            .HasForeignKey(a => a.JobPostId)
            .OnDelete(DeleteBehavior.Cascade);

        // Candidate
        modelBuilder.Entity<Candidate>()
            .HasMany(c => c.Applications)
            .WithOne(a => a.Candidate)
            .HasForeignKey(a => a.CandidateId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        modelBuilder.Entity<Company>()
            .HasIndex(c => c.Email)
            .IsUnique();

        modelBuilder.Entity<Candidate>()
            .HasIndex(c => c.Email)
            .IsUnique();

        modelBuilder.Entity<JobPost>()
            .HasIndex(j => j.PostedAt);

        modelBuilder.Entity<JobApplication>()
            .HasIndex(j => j.AppliedAt);
    }
} 