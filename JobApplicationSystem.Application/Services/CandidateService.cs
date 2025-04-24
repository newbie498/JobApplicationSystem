using AutoMapper;
using JobApplicationSystem.Core.DTOs;
using JobApplicationSystem.Core.Entities;
using JobApplicationSystem.Core.Interfaces;

namespace JobApplicationSystem.Application.Services;

public class CandidateService : ICandidateService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CandidateService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Candidate> RegisterCandidateAsync(CreateCandidateDto createCandidateDto)
    {
        var existingCandidate = await _unitOfWork.Candidates.GetByEmailAsync(createCandidateDto.Email);
        if (existingCandidate != null)
        {
            throw new InvalidOperationException("Email already registered");
        }

        var candidate = _mapper.Map<Candidate>(createCandidateDto);
        await _unitOfWork.Candidates.AddAsync(candidate);
        await _unitOfWork.SaveChangesAsync();
        return candidate;
    }

    public async Task<Candidate?> GetCandidateByIdAsync(int id)
    {
        return await _unitOfWork.Candidates.GetByIdAsync(id);
    }

    public async Task<CandidateDto?> GetCandidateByEmailAsync(string email)
    {
        var candidate = await _unitOfWork.Candidates.GetByEmailAsync(email);
        return candidate != null ? _mapper.Map<CandidateDto>(candidate) : null;
    }

    public async Task<CandidateDto> UpdateCandidateAsync(int id, UpdateCandidateDto updateCandidateDto)
    {
        var candidate = await _unitOfWork.Candidates.GetByIdAsync(id);
        if (candidate == null)
        {
            throw new KeyNotFoundException($"Candidate with ID {id} not found");
        }

        _mapper.Map(updateCandidateDto, candidate);
        await _unitOfWork.Candidates.UpdateAsync(candidate);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<CandidateDto>(candidate);
    }

    public async Task<bool> DeleteCandidateAsync(int id)
    {
        var candidate = await _unitOfWork.Candidates.GetByIdAsync(id);
        if (candidate == null)
        {
            return false;
        }

        await _unitOfWork.Candidates.DeleteAsync(candidate);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<JobApplicationDto>> GetCandidateApplicationsAsync(int candidateId)
    {
        var applications = await _unitOfWork.Candidates.GetApplicationsAsync(candidateId);
        return _mapper.Map<IEnumerable<JobApplicationDto>>(applications);
    }
} 