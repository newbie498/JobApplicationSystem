using AutoMapper;
using Moq;
using Xunit;
using JobApplicationSystem.Application.Services;
using JobApplicationSystem.Core.DTOs;
using JobApplicationSystem.Core.Entities;
using JobApplicationSystem.Core.Interfaces;

namespace JobApplicationSystem.Tests.Services;

public class CandidateServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly CandidateService _service;

    public CandidateServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _service = new CandidateService(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task RegisterCandidateAsync_ValidData_ReturnsCandidate()
    {
        // Arrange
        var createDto = new CreateCandidateDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Phone = "1234567890",
            ResumeUrl = "https://example.com/resume.pdf",
            Skills = new List<string> { "C#", ".NET" }
        };

        var candidate = new Candidate
        {
            Id = 1,
            FirstName = createDto.FirstName,
            LastName = createDto.LastName,
            Email = createDto.Email
        };

        _mockUnitOfWork.Setup(u => u.Candidates.GetByEmailAsync(createDto.Email))
            .ReturnsAsync((Candidate?)null);
        _mockMapper.Setup(m => m.Map<Candidate>(createDto))
            .Returns(candidate);
        _mockUnitOfWork.Setup(u => u.Candidates.AddAsync(It.IsAny<Candidate>()))
            .ReturnsAsync(candidate);

        // Act
        var result = await _service.RegisterCandidateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(candidate.Id, result.Id);
        Assert.Equal(candidate.Email, result.Email);
    }

    [Fact]
    public async Task RegisterCandidateAsync_DuplicateEmail_ThrowsInvalidOperationException()
    {
        // Arrange
        var createDto = new CreateCandidateDto
        {
            Email = "existing@example.com"
        };

        var existingCandidate = new Candidate { Email = createDto.Email };

        _mockUnitOfWork.Setup(u => u.Candidates.GetByEmailAsync(createDto.Email))
            .ReturnsAsync(existingCandidate);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.RegisterCandidateAsync(createDto));
    }

    [Fact]
    public async Task GetCandidateByIdAsync_ValidId_ReturnsCandidate()
    {
        // Arrange
        var candidateId = 1;
        var candidate = new Candidate
        {
            Id = candidateId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };

        _mockUnitOfWork.Setup(u => u.Candidates.GetByIdAsync(candidateId))
            .ReturnsAsync(candidate);

        // Act
        var result = await _service.GetCandidateByIdAsync(candidateId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(candidate.Id, result.Id);
        Assert.Equal(candidate.Email, result.Email);
    }

    [Fact]
    public async Task GetCandidateByEmailAsync_ValidEmail_ReturnsCandidateDto()
    {
        // Arrange
        var email = "john.doe@example.com";
        var candidate = new Candidate
        {
            Id = 1,
            Email = email,
            FirstName = "John",
            LastName = "Doe"
        };
        var expectedDto = new CandidateDto
        {
            Id = 1,
            Email = email,
            FirstName = "John",
            LastName = "Doe"
        };

        _mockUnitOfWork.Setup(u => u.Candidates.GetByEmailAsync(email))
            .ReturnsAsync(candidate);
        _mockMapper.Setup(m => m.Map<CandidateDto>(candidate))
            .Returns(expectedDto);

        // Act
        var result = await _service.GetCandidateByEmailAsync(email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedDto.Email, result.Email);
    }

    [Fact]
    public async Task UpdateCandidateAsync_ValidData_ReturnsCandidateDto()
    {
        // Arrange
        var candidateId = 1;
        var updateDto = new UpdateCandidateDto
        {
            FirstName = "John",
            LastName = "Doe",
            Phone = "1234567890",
            ResumeUrl = "https://example.com/resume.pdf",
            Skills = new List<string> { "C#", ".NET" }
        };

        var existingCandidate = new Candidate { Id = candidateId };
        var updatedCandidate = new Candidate
        {
            Id = candidateId,
            FirstName = updateDto.FirstName,
            LastName = updateDto.LastName
        };
        var expectedDto = new CandidateDto
        {
            Id = candidateId,
            FirstName = updateDto.FirstName,
            LastName = updateDto.LastName
        };

        _mockUnitOfWork.Setup(u => u.Candidates.GetByIdAsync(candidateId))
            .ReturnsAsync(existingCandidate);
        _mockMapper.Setup(m => m.Map(updateDto, existingCandidate))
            .Returns(updatedCandidate);
        _mockMapper.Setup(m => m.Map<CandidateDto>(It.IsAny<Candidate>()))
            .Returns(expectedDto);

        // Act
        var result = await _service.UpdateCandidateAsync(candidateId, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedDto.Id, result.Id);
        Assert.Equal(expectedDto.FirstName, result.FirstName);
    }

    [Fact]
    public async Task UpdateCandidateAsync_InvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var candidateId = 1;
        var updateDto = new UpdateCandidateDto();

        _mockUnitOfWork.Setup(u => u.Candidates.GetByIdAsync(candidateId))
            .ReturnsAsync((Candidate?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            _service.UpdateCandidateAsync(candidateId, updateDto));
    }

    [Fact]
    public async Task DeleteCandidateAsync_ValidId_ReturnsTrue()
    {
        // Arrange
        var candidateId = 1;
        var candidate = new Candidate { Id = candidateId };

        _mockUnitOfWork.Setup(u => u.Candidates.GetByIdAsync(candidateId))
            .ReturnsAsync(candidate);
        _mockUnitOfWork.Setup(u => u.Candidates.DeleteAsync(candidate))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteCandidateAsync(candidateId);

        // Assert
        Assert.True(result);
        _mockUnitOfWork.Verify(u => u.Candidates.DeleteAsync(candidate), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteCandidateAsync_InvalidId_ReturnsFalse()
    {
        // Arrange
        var candidateId = 1;

        _mockUnitOfWork.Setup(u => u.Candidates.GetByIdAsync(candidateId))
            .ReturnsAsync((Candidate?)null);

        // Act
        var result = await _service.DeleteCandidateAsync(candidateId);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(u => u.Candidates.DeleteAsync(It.IsAny<Candidate>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
} 