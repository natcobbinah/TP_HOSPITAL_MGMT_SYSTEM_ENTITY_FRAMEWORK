using HospitalManagement.Application.Common;
using HospitalManagement.Application.DTOs;
using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Application.Services;

public class PatientService : IPatientService
{
    private readonly IUnitOfWork _unitOfWork;

    public PatientService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Creates a new patient after validating:
    /// - Date of birth is in the past
    /// - File number is unique
    /// - Email is unique
    /// </summary>
    public async Task<PatientDto> CreateAsync(CreatePatientDto dto)
    {
        // Validate date of birth is in the past
        if (dto.DateOfBirth >= DateTime.UtcNow)
            throw new ArgumentException("Date of birth must be in the past.");

        // Check unique file number
        if (await _unitOfWork.Patients.FileNumberExistsAsync(dto.FileNumber))
            throw new InvalidOperationException($"File number '{dto.FileNumber}' already exists.");

        // Check unique email
        if (await _unitOfWork.Patients.EmailExistsAsync(dto.Email))
            throw new InvalidOperationException($"Email '{dto.Email}' already exists.");

        var patient = new Patient
        {
            FileNumber = dto.FileNumber,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            DateOfBirth = dto.DateOfBirth,
            Phone = dto.Phone,
            Email = dto.Email,
            Address = new Address
            {
                Street = dto.Street,
                City = dto.City,
                ZipCode = dto.ZipCode,
                Country = dto.Country
            }
        };

        await _unitOfWork.Patients.AddAsync(patient);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(patient);
    }

    /// <summary>
    /// Updates patient information.
    /// Handles concurrency conflicts via RowVersion (optimistic concurrency).
    /// </summary>
    public async Task<PatientDto> UpdateAsync(int id, UpdatePatientDto dto)
    {
        var patient = await _unitOfWork.Patients.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Patient with ID {id} not found.");

        if (await _unitOfWork.Patients.EmailExistsAsync(dto.Email, id))
            throw new InvalidOperationException($"Email '{dto.Email}' is already used by another patient.");

        patient.FirstName = dto.FirstName;
        patient.LastName = dto.LastName;
        patient.Phone = dto.Phone;
        patient.Email = dto.Email;
        patient.Address = new Address
        {
            Street = dto.Street,
            City = dto.City,
            ZipCode = dto.ZipCode,
            Country = dto.Country
        };

        _unitOfWork.Patients.Update(patient);

        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // This still fires when ConcurrencyStamp mismatches
            throw new InvalidOperationException(
                "The patient record was modified by another user. Please reload and try again.");
        }

        return MapToDto(patient);
    }

    /// <summary>
    /// Deletes a patient. If the patient has consultations, EF Core will cascade delete them
    /// (configured in ConsultationConfiguration with DeleteBehavior.Cascade on Patient side).
    /// </summary>
    public async Task DeleteAsync(int id)
    {
        var patient = await _unitOfWork.Patients.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Patient with ID {id} not found.");

        try
        {
            _unitOfWork.Patients.Delete(patient);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException(
                $"Cannot delete patient '{patient.FirstName} {patient.LastName}'. " +
                $"There may be related records preventing deletion. Details: {ex.InnerException?.Message}");
        }
    }

    /// <summary>
    /// Gets a single patient by ID (without related data).
    /// Uses repository which calls FindAsync — tracked by default.
    /// </summary>
    public async Task<PatientDto?> GetByIdAsync(int id)
    {
        var patient = await _unitOfWork.Patients.GetByIdAsync(id);
        return patient is null ? null : MapToDto(patient);
    }

    /// <summary>
    /// Step 5 - View 1: Full patient detail with all consultations and doctor names.
    /// Uses Eager Loading (Include + ThenInclude) via repository.
    /// </summary>
    public async Task<PatientDetailDto?> GetDetailAsync(int id)
    {
        var patient = await _unitOfWork.Patients.GetByIdWithConsultationsAsync(id);

        if (patient is null) return null;

        return new PatientDetailDto
        {
            Id = patient.Id,
            FileNumber = patient.FileNumber,
            FullName = $"{patient.FirstName} {patient.LastName}",
            DateOfBirth = patient.DateOfBirth,
            Email = patient.Email,
            Phone = patient.Phone,
            Consultations = patient.Consultations
                .OrderByDescending(c => c.Date)
                .Select(c => new ConsultationDto
                {
                    Id = c.Id,
                    Date = c.Date,
                    Status = c.Status.ToString(),
                    Notes = c.Notes,
                    Reason = c.Reason,
                    PatientId = c.PatientId,
                    PatientName = $"{patient.FirstName} {patient.LastName}",
                    DoctorId = c.DoctorId,
                    DoctorName = $"Dr. {c.Doctor.FirstName} {c.Doctor.LastName}"
                }).ToList()
        };
    }

    /// <summary>
    /// Paginated listing of all patients, ordered alphabetically.
    /// Uses AsNoTracking for read-only performance.
    /// </summary>
    public async Task<PagedResult<PatientDto>> GetAllAsync(int page = 1, int pageSize = 20)
    {
        var patients = await _unitOfWork.Patients.GetAllAsync(page, pageSize);
        var totalCount = await _unitOfWork.Patients.GetTotalCountAsync();

        return new PagedResult<PatientDto>
        {
            Items = patients.Select(MapToDto),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Search patients by first or last name with pagination.
    /// The repository uses LIKE/Contains which leverages the IX_Patients_LastName index.
    /// </summary>
    public async Task<PagedResult<PatientDto>> SearchByNameAsync(string name, int page = 1, int pageSize = 20)
    {
        if (string.IsNullOrWhiteSpace(name))
            return await GetAllAsync(page, pageSize);

        var patients = await _unitOfWork.Patients.SearchByNameAsync(name, page, pageSize);
        var totalCount = await _unitOfWork.Patients.GetSearchCountAsync(name);

        return new PagedResult<PatientDto>
        {
            Items = patients.Select(MapToDto),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    // ──────────────────────────────────────────────
    // Private helper: Entity → DTO mapping
    // ──────────────────────────────────────────────

    private static PatientDto MapToDto(Patient patient)
    {
        return new PatientDto
        {
            Id = patient.Id,
            FileNumber = patient.FileNumber,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            DateOfBirth = patient.DateOfBirth,
            Phone = patient.Phone,
            Email = patient.Email,
            Street = patient.Address?.Street ?? string.Empty,
            City = patient.Address?.City ?? string.Empty,
            ZipCode = patient.Address?.ZipCode ?? string.Empty,
            Country = patient.Address?.Country ?? string.Empty
        };
    }
}