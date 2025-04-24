namespace JobApplicationSystem.Core.DTOs;

public class JobPostSearchDto
{
    public string? Title { get; set; }
    public string? Location { get; set; }
    public string? CompanyName { get; set; }
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public bool? IsActive { get; set; }
    public string? Keyword { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
} 