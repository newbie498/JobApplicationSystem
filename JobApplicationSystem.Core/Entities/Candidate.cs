using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JobApplicationSystem.Core.Entities;

public class Candidate
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

    [Required]
    public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

    [Required]
    public string ResumeUrl { get; set; } = string.Empty;

    public string? LinkedInProfile { get; set; }

    public string? PortfolioUrl { get; set; }

    public List<string> Skills { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
} 