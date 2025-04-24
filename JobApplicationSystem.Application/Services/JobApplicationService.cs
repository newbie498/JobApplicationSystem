using AutoMapper;
using JobApplicationSystem.Core.DTOs;
using JobApplicationSystem.Core.Entities;
using JobApplicationSystem.Core.Interfaces;

namespace JobApplicationSystem.Application.Services;

public class JobApplicationService : IJobApplicationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public JobApplicationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<JobApplication> SubmitApplicationAsync(CreateJobApplicationDto createJobApplicationDto)
    {
        var jobPost = await _unitOfWork.JobPosts.GetByIdAsync(createJobApplicationDto.JobPostId);
        if (jobPost == null)
        {
            throw new KeyNotFoundException("Job post not found");
        }

        var candidate = await _unitOfWork.Candidates.GetByIdAsync(createJobApplicationDto.CandidateId);
        if (candidate == null)
        {
            throw new KeyNotFoundException("Candidate not found");
        }

        var existingApplication = await _unitOfWork.JobApplications.ExistsAsync(
            createJobApplicationDto.CandidateId,
            createJobApplicationDto.JobPostId);

        if (existingApplication)
        {
            throw new InvalidOperationException("You have already applied to this job");
        }

        var application = new JobApplication
        {
            JobPostId = createJobApplicationDto.JobPostId,
            CandidateId = createJobApplicationDto.CandidateId,
            CoverLetter = createJobApplicationDto.CoverLetter,
            AdditionalNotes = createJobApplicationDto.AdditionalNotes,
            Status = ApplicationStatus.Pending,
            AppliedAt = DateTime.UtcNow
        };

        await _unitOfWork.JobApplications.AddAsync(application);
        await _unitOfWork.SaveChangesAsync();

        return application;
    }

    public async Task<JobApplication?> GetApplicationByIdAsync(int id)
    {
        return await _unitOfWork.JobApplications.GetByIdWithDetailsAsync(id);
    }

    public async Task<JobApplicationDto> CreateJobApplicationAsync(int candidateId, CreateJobApplicationDto createJobApplicationDto)
    {
        createJobApplicationDto.CandidateId = candidateId;
        var application = await SubmitApplicationAsync(createJobApplicationDto);
        return _mapper.Map<JobApplicationDto>(application);
    }

    public async Task<JobApplicationDto?> GetJobApplicationByIdAsync(int id)
    {
        var application = await GetApplicationByIdAsync(id);
        return application != null ? _mapper.Map<JobApplicationDto>(application) : null;
    }

    public async Task<JobApplicationDto> UpdateJobApplicationStatusAsync(int id, UpdateJobApplicationStatusDto updateStatusDto)
    {
        var application = await _unitOfWork.JobApplications.GetByIdAsync(id);
        if (application == null)
        {
            throw new KeyNotFoundException($"Job application with ID {id} not found");
        }

        // Get all valid status values
        var validStatuses = Enum.GetNames(typeof(ApplicationStatus));
        var providedStatus = updateStatusDto.Status.ToString();

        // Try to find a matching status (case-insensitive)
        var matchingStatus = validStatuses.FirstOrDefault(s =>
            string.Equals(s, providedStatus, StringComparison.OrdinalIgnoreCase));

        if (matchingStatus == null)
        {
            throw new ArgumentException($"Invalid application status: {updateStatusDto.Status}. Valid statuses are: {string.Join(", ", validStatuses)}");
        }

        application.Status = (ApplicationStatus)Enum.Parse(typeof(ApplicationStatus), matchingStatus);
        await _unitOfWork.JobApplications.UpdateAsync(application);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<JobApplicationDto>(application);
    }

    public async Task<bool> DeleteJobApplicationAsync(int id)
    {
        var application = await _unitOfWork.JobApplications.GetByIdAsync(id);
        if (application == null)
        {
            return false;
        }

        await _unitOfWork.JobApplications.DeleteAsync(application);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<JobApplicationDto>> GetApplicationsByJobPostAsync(int jobPostId)
    {
        var jobPost = await _unitOfWork.JobPosts.GetByIdAsync(jobPostId);
        if (jobPost == null)
        {
            throw new KeyNotFoundException($"Job post with ID {jobPostId} not found");
        }

        var applications = await _unitOfWork.JobApplications.GetByJobPostIdAsync(jobPostId);
        return _mapper.Map<IEnumerable<JobApplicationDto>>(applications);
    }

    public async Task<IEnumerable<JobApplicationDto>> GetApplicationsByCandidateAsync(int candidateId)
    {
        var candidate = await _unitOfWork.Candidates.GetByIdAsync(candidateId);
        if (candidate == null)
        {
            throw new KeyNotFoundException($"Candidate with ID {candidateId} not found");
        }

        var applications = await _unitOfWork.JobApplications.GetByCandidateIdAsync(candidateId);
        return _mapper.Map<IEnumerable<JobApplicationDto>>(applications);
    }
}