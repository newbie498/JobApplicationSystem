using System;
using System.ComponentModel.DataAnnotations;

namespace JobApplicationSystem.Core.Entities;

public class JobApplication
{
    public int Id { get; set; }

    [Required]
    public int JobPostId { get; set; }
    public virtual JobPost JobPost { get; set; } = null!;

    [Required]
    public int CandidateId { get; set; }
    public virtual Candidate Candidate { get; set; } = null!;

    [Required]
    public string CoverLetter { get; set; } = string.Empty;

    public string? AdditionalNotes { get; set; }

    [Required]
    public ApplicationStatus Status { get; set; }

    [Required]
    public DateTime AppliedAt { get; set; }
}

public enum ApplicationStatus
{
    Pending,
    UnderReview,
    Shortlisted,
    Rejected,
    Accepted
} 