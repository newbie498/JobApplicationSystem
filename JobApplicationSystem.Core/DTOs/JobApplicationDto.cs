using System.ComponentModel.DataAnnotations;
using JobApplicationSystem.Core.Entities;

namespace JobApplicationSystem.Core.DTOs;

public class JobApplicationDto
{
    public int Id { get; set; }
    public int CandidateId { get; set; }
    public CandidateDto Candidate { get; set; } = null!;
    public int JobPostId { get; set; }
    public JobPostDto JobPost { get; set; } = null!;
    public string ResumeUrl { get; set; } = string.Empty;
    public string? CoverLetter { get; set; }
    public DateTime AppliedAt { get; set; }
    public ApplicationStatus Status { get; set; }
}

public class CreateJobApplicationDto
{
    public int JobPostId { get; set; }
    public int CandidateId { get; set; }
    public string CoverLetter { get; set; } = string.Empty;
    public string? AdditionalNotes { get; set; }
}
public class UpdateJobApplicationStatusDto
{
    public ApplicationStatus Status { get; set; }
}