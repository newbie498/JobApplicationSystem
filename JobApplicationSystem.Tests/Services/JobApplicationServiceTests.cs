using AutoMapper;
using Moq;
using Xunit;
using JobApplicationSystem.Application.Services;
using JobApplicationSystem.Core.DTOs;
using JobApplicationSystem.Core.Entities;
using JobApplicationSystem.Core.Interfaces;

namespace JobApplicationSystem.Tests.Services;

public class JobApplicationServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly JobApplicationService _service;

    public JobApplicationServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _service = new JobApplicationService(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task SubmitApplicationAsync_ValidData_ReturnsJobApplication()
    {
        // Arrange
        var createDto = new CreateJobApplicationDto
        {
            JobPostId = 1,
            CandidateId = 1,
            CoverLetter = "Test cover letter",
            AdditionalNotes = "Test notes"
        };

        var jobPost = new JobPost { Id = 1 };
        var candidate = new Candidate { Id = 1 };
        var expectedApplication = new JobApplication
        {
            JobPostId = 1,
            CandidateId = 1,
            CoverLetter = "Test cover letter",
            AdditionalNotes = "Test notes",
            Status = ApplicationStatus.Pending
        };

        _mockUnitOfWork.Setup(u => u.JobPosts.GetByIdAsync(1))
            .ReturnsAsync(jobPost);
        _mockUnitOfWork.Setup(u => u.Candidates.GetByIdAsync(1))
            .ReturnsAsync(candidate);
        _mockUnitOfWork.Setup(u => u.JobApplications.ExistsAsync(1, 1))
            .ReturnsAsync(false);
        _mockUnitOfWork.Setup(u => u.JobApplications.AddAsync(It.IsAny<JobApplication>()))
            .ReturnsAsync(expectedApplication);

        // Act
        var result = await _service.SubmitApplicationAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedApplication.JobPostId, result.JobPostId);
        Assert.Equal(expectedApplication.CandidateId, result.CandidateId);
        Assert.Equal(expectedApplication.CoverLetter, result.CoverLetter);
        Assert.Equal(expectedApplication.AdditionalNotes, result.AdditionalNotes);
        Assert.Equal(ApplicationStatus.Pending, result.Status);
    }

    [Fact]
    public async Task SubmitApplicationAsync_JobPostNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var createDto = new CreateJobApplicationDto { JobPostId = 1 };
        _mockUnitOfWork.Setup(u => u.JobPosts.GetByIdAsync(1))
            .ReturnsAsync((JobPost?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            _service.SubmitApplicationAsync(createDto));
    }

    [Fact]
    public async Task SubmitApplicationAsync_CandidateNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var createDto = new CreateJobApplicationDto 
        { 
            JobPostId = 1,
            CandidateId = 1
        };
        var jobPost = new JobPost { Id = 1 };
        
        _mockUnitOfWork.Setup(u => u.JobPosts.GetByIdAsync(1))
            .ReturnsAsync(jobPost);
        _mockUnitOfWork.Setup(u => u.Candidates.GetByIdAsync(1))
            .ReturnsAsync((Candidate?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            _service.SubmitApplicationAsync(createDto));
    }

    [Fact]
    public async Task SubmitApplicationAsync_ApplicationExists_ThrowsInvalidOperationException()
    {
        // Arrange
        var createDto = new CreateJobApplicationDto 
        { 
            JobPostId = 1,
            CandidateId = 1
        };
        var jobPost = new JobPost { Id = 1 };
        var candidate = new Candidate { Id = 1 };

        _mockUnitOfWork.Setup(u => u.JobPosts.GetByIdAsync(1))
            .ReturnsAsync(jobPost);
        _mockUnitOfWork.Setup(u => u.Candidates.GetByIdAsync(1))
            .ReturnsAsync(candidate);
        _mockUnitOfWork.Setup(u => u.JobApplications.ExistsAsync(1, 1))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.SubmitApplicationAsync(createDto));
    }

    [Fact]
    public async Task UpdateJobApplicationStatusAsync_ValidStatus_ReturnsUpdatedDto()
    {
        // Arrange
        var application = new JobApplication { Id = 1, Status = ApplicationStatus.Pending };
        var updateDto = new UpdateJobApplicationStatusDto { Status = ApplicationStatus.UnderReview };
        var expectedDto = new JobApplicationDto { Id = 1, Status = ApplicationStatus.UnderReview };

        _mockUnitOfWork.Setup(u => u.JobApplications.GetByIdAsync(1))
            .ReturnsAsync(application);
        _mockMapper.Setup(m => m.Map<JobApplicationDto>(It.IsAny<JobApplication>()))
            .Returns(expectedDto);

        // Act
        var result = await _service.UpdateJobApplicationStatusAsync(1, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedDto.Status, result.Status);
    }

    [Fact]
    public async Task UpdateJobApplicationStatusAsync_InvalidStatus_ThrowsArgumentException()
    {
        // Arrange
        var application = new JobApplication { Id = 1 };
        var updateDto = new UpdateJobApplicationStatusDto { Status = (ApplicationStatus)999 };

        _mockUnitOfWork.Setup(u => u.JobApplications.GetByIdAsync(1))
            .ReturnsAsync(application);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.UpdateJobApplicationStatusAsync(1, updateDto));
    }

    [Fact]
    public async Task GetApplicationsByJobPostAsync_ValidId_ReturnsApplications()
    {
        // Arrange
        var jobPost = new JobPost { Id = 1 };
        var applications = new List<JobApplication>
        {
            new() { Id = 1, JobPostId = 1 },
            new() { Id = 2, JobPostId = 1 }
        };
        var expectedDtos = new List<JobApplicationDto>
        {
            new() { Id = 1, JobPostId = 1 },
            new() { Id = 2, JobPostId = 1 }
        };

        _mockUnitOfWork.Setup(u => u.JobPosts.GetByIdAsync(1))
            .ReturnsAsync(jobPost);
        _mockUnitOfWork.Setup(u => u.JobApplications.GetByJobPostIdAsync(1))
            .ReturnsAsync(applications);
        _mockMapper.Setup(m => m.Map<IEnumerable<JobApplicationDto>>(applications))
            .Returns(expectedDtos);

        // Act
        var result = await _service.GetApplicationsByJobPostAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetApplicationsByCandidateAsync_ValidId_ReturnsApplications()
    {
        // Arrange
        var candidate = new Candidate { Id = 1 };
        var applications = new List<JobApplication>
        {
            new() { Id = 1, CandidateId = 1 },
            new() { Id = 2, CandidateId = 1 }
        };
        var expectedDtos = new List<JobApplicationDto>
        {
            new() { Id = 1, CandidateId = 1 },
            new() { Id = 2, CandidateId = 1 }
        };

        _mockUnitOfWork.Setup(u => u.Candidates.GetByIdAsync(1))
            .ReturnsAsync(candidate);
        _mockUnitOfWork.Setup(u => u.JobApplications.GetByCandidateIdAsync(1))
            .ReturnsAsync(applications);
        _mockMapper.Setup(m => m.Map<IEnumerable<JobApplicationDto>>(applications))
            .Returns(expectedDtos);

        // Act
        var result = await _service.GetApplicationsByCandidateAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
} 