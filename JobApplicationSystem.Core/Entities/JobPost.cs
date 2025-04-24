using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JobApplicationSystem.Core.Entities;

public class JobPost
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime PostedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime ExpiresAt { get; set; }

    [Required]
    public string Location { get; set; } = string.Empty;

    [Required]
    public string JobType { get; set; } = string.Empty; // Full-time, Part-time, Contract

    public decimal? SalaryRangeStart { get; set; }
    public decimal? SalaryRangeEnd { get; set; }

    [Required]
    public int CompanyId { get; set; }
    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();

    public decimal Salary { get; set; }
    public DateTime PostedDate { get; set; }
    public DateTime? ClosingDate { get; set; }
    public bool IsActive { get; set; }
} 