using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Xunit;
using JobApplicationSystem.Core.DTOs;
using JobApplicationSystem.Core.Entities;
using JobApplicationSystem.Core.Interfaces;
using JobApplicationSystem.Infrastructure.Data;
using JobApplicationSystem.Infrastructure.Mapping;
using JobApplicationSystem.Application.Services;

namespace JobApplicationSystem.Tests.Services;

public class CompanyServiceTests
{
    private readonly DbContextOptions<ApplicationDbContext> _options;
    private readonly IMapper _mapper;

    public CompanyServiceTests()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public async Task CreateCompany_ShouldCreateAndReturnCompany()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var unitOfWork = new UnitOfWork(context);
        var service = new CompanyService(unitOfWork, _mapper);
        var createDto = new CreateCompanyDto
        {
            Name = "Test Company",
            Email = "test@company.com",
            Description = "Test Description"
        };

        // Act
        var result = await service.CreateCompanyAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createDto.Name, result.Name);
        Assert.Equal(createDto.Email, result.Email);
        Assert.Equal(createDto.Description, result.Description);
    }

    [Fact]
    public async Task GetCompanyById_ShouldReturnCompany_WhenExists()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        var unitOfWork = new UnitOfWork(context);
        var service = new CompanyService(unitOfWork, _mapper);
        var company = new Company
        {
            Name = "Test Company",
            Email = "test@company.com",
            Description = "Test Description"
        };
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetCompanyByIdAsync(company.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(company.Name, result.Name);
        Assert.Equal(company.Email, result.Email);
        Assert.Equal(company.Description, result.Description);
    }
}