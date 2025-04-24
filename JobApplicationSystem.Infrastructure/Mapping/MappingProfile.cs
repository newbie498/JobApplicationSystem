using AutoMapper;
using JobApplicationSystem.Core.DTOs;
using JobApplicationSystem.Core.Entities;

namespace JobApplicationSystem.Infrastructure.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Company mappings
        CreateMap<Company, CompanyDto>();
        CreateMap<CreateCompanyDto, Company>();
        CreateMap<UpdateCompanyDto, Company>();

        // JobPost mappings
        CreateMap<JobPost, JobPostDto>();
        CreateMap<CreateJobPostDto, JobPost>();
        CreateMap<UpdateJobPostDto, JobPost>();

        // Candidate mappings
        CreateMap<Candidate, CandidateDto>();
        CreateMap<CreateCandidateDto, Candidate>();
        CreateMap<UpdateCandidateDto, Candidate>();

        // JobApplication mappings
        CreateMap<JobApplication, JobApplicationDto>();
        CreateMap<CreateJobApplicationDto, JobApplication>();
    }
} 