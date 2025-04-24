namespace JobApplicationSystem.Core.DTOs;

public class JobPostDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime PostedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string Location { get; set; } = string.Empty;
    public string JobType { get; set; } = string.Empty;
    public decimal? SalaryRangeStart { get; set; }
    public decimal? SalaryRangeEnd { get; set; }
    public int CompanyId { get; set; }
    public CompanyDto Company { get; set; } = null!;
}

public class CreateJobPostDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string Location { get; set; } = string.Empty;
    public string JobType { get; set; } = string.Empty;
    public decimal? SalaryRangeStart { get; set; }
    public decimal? SalaryRangeEnd { get; set; }
}

public class UpdateJobPostDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string Location { get; set; } = string.Empty;
    public string JobType { get; set; } = string.Empty;
    public decimal? SalaryRangeStart { get; set; }
    public decimal? SalaryRangeEnd { get; set; }
}