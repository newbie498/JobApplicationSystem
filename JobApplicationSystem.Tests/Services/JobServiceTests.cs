using AutoMapper;
using Moq;
using Xunit;
using JobApplicationSystem.Application.Services;
using JobApplicationSystem.Core.DTOs;
using JobApplicationSystem.Core.Entities;
using JobApplicationSystem.Core.Interfaces;

namespace JobApplicationSystem.Tests.Services;

public class JobServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly JobService _service;

    public JobServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _service = new JobService(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task CreateJobAsync_ValidData_ReturnsJobPostDto()
    {
        // Arrange
        var companyId = 1;
        var createDto = new CreateJobPostDto
        {
            Title = "Software Developer",
            Description = "Job description",
            Location = "Remote",
            JobType = "Full-time",
            SalaryRangeStart = 80000,
            SalaryRangeEnd = 120000
        };

        var company = new Company { Id = companyId };
        var jobPost = new JobPost
        {
            Id = 1,
            Title = createDto.Title,
            Description = createDto.Description,
            CompanyId = companyId
        };
        var expectedDto = new JobPostDto
        {
            Id = 1,
            Title = createDto.Title,
            Description = createDto.Description
        };

        _mockUnitOfWork.Setup(u => u.Companies.GetByIdAsync(companyId))
            .ReturnsAsync(company);
        _mockMapper.Setup(m => m.Map<JobPost>(createDto))
            .Returns(jobPost);
        _mockUnitOfWork.Setup(u => u.JobPosts.AddAsync(It.IsAny<JobPost>()))
            .ReturnsAsync(jobPost);
        _mockMapper.Setup(m => m.Map<JobPostDto>(jobPost))
            .Returns(expectedDto);

        // Act
        var result = await _service.CreateJobAsync(companyId, createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedDto.Id, result.Id);
        Assert.Equal(expectedDto.Title, result.Title);
        Assert.Equal(expectedDto.Description, result.Description);
    }

    [Fact]
    public async Task CreateJobAsync_CompanyNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var companyId = 1;
        var createDto = new CreateJobPostDto();

        _mockUnitOfWork.Setup(u => u.Companies.GetByIdAsync(companyId))
            .ReturnsAsync((Company?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            _service.CreateJobAsync(companyId, createDto));
    }

    [Fact]
    public async Task GetJobByIdAsync_ValidId_ReturnsJobPostDto()
    {
        // Arrange
        var jobId = 1;
        var jobPost = new JobPost { Id = jobId, Title = "Software Developer" };
        var expectedDto = new JobPostDto { Id = jobId, Title = "Software Developer" };

        _mockUnitOfWork.Setup(u => u.JobPosts.GetByIdWithDetailsAsync(jobId))
            .ReturnsAsync(jobPost);
        _mockMapper.Setup(m => m.Map<JobPostDto>(jobPost))
            .Returns(expectedDto);

        // Act
        var result = await _service.GetJobByIdAsync(jobId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedDto.Id, result.Id);
        Assert.Equal(expectedDto.Title, result.Title);
    }

    [Fact]
    public async Task GetJobByIdAsync_InvalidId_ReturnsNull()
    {
        // Arrange
        var jobId = 1;
        _mockUnitOfWork.Setup(u => u.JobPosts.GetByIdWithDetailsAsync(jobId))
            .ReturnsAsync((JobPost?)null);

        // Act
        var result = await _service.GetJobByIdAsync(jobId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SearchJobsAsync_WithParameters_ReturnsFilteredJobs()
    {
        // Arrange
        var keyword = "developer";
        var company = "tech";
        var fromDate = DateTime.UtcNow.AddDays(-7);
        var toDate = DateTime.UtcNow;

        var jobPosts = new List<JobPost>
        {
            new() { Id = 1, Title = "Software Developer" },
            new() { Id = 2, Title = "Senior Developer" }
        };
        var expectedDtos = new List<JobPostDto>
        {
            new() { Id = 1, Title = "Software Developer" },
            new() { Id = 2, Title = "Senior Developer" }
        };

        _mockUnitOfWork.Setup(u => u.JobPosts.SearchJobPostsAsync(It.IsAny<JobPostSearchDto>()))
            .ReturnsAsync(jobPosts);
        _mockMapper.Setup(m => m.Map<IEnumerable<JobPostDto>>(jobPosts))
            .Returns(expectedDtos);

        // Act
        var result = await _service.SearchJobsAsync(keyword, company, fromDate, toDate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, j => j.Title.Contains("Developer"));
    }

    [Fact]
    public async Task SearchJobsAsync_NoParameters_ReturnsAllJobs()
    {
        // Arrange
        var jobPosts = new List<JobPost>
        {
            new() { Id = 1, Title = "Software Developer" },
            new() { Id = 2, Title = "Project Manager" }
        };
        var expectedDtos = new List<JobPostDto>
        {
            new() { Id = 1, Title = "Software Developer" },
            new() { Id = 2, Title = "Project Manager" }
        };

        _mockUnitOfWork.Setup(u => u.JobPosts.SearchJobPostsAsync(It.IsAny<JobPostSearchDto>()))
            .ReturnsAsync(jobPosts);
        _mockMapper.Setup(m => m.Map<IEnumerable<JobPostDto>>(jobPosts))
            .Returns(expectedDtos);

        // Act
        var result = await _service.SearchJobsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
} 