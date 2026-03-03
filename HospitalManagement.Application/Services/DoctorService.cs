using HospitalManagement.Application.DTOs;
using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Enums;
using HospitalManagement.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Application.Services;

public class DoctorService : IDoctorService
{
    private readonly IUnitOfWork _unitOfWork;

    public DoctorService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Creates a new doctor and assigns them to a department.
    /// Validates department existence and specialty enum.
    /// </summary>
    public async Task<DoctorDto> CreateAsync(CreateDoctorDto dto)
    {
        // Validate department exists
        var department = await _unitOfWork.Departments.GetByIdAsync(dto.DepartmentId)
            ?? throw new KeyNotFoundException($"Department with ID {dto.DepartmentId} not found.");

        // Validate specialty enum
        if (!Enum.TryParse<Specialty>(dto.Specialty, ignoreCase: true, out var specialty))
            throw new ArgumentException(
                $"Invalid specialty '{dto.Specialty}'. Valid values: {string.Join(", ", Enum.GetNames<Specialty>())}");

        var doctor = new Doctor
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            LicenseNumber = dto.LicenseNumber,
            Specialty = specialty,
            DepartmentId = dto.DepartmentId,
            HireDate = DateTime.UtcNow
        };

        await _unitOfWork.Doctors.AddAsync(doctor);

        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") == true)
        {
            throw new InvalidOperationException(
                $"License number '{dto.LicenseNumber}' already exists.");
        }

        return MapToDto(doctor, department.Name);
    }

    public async Task<DoctorDto?> GetByIdAsync(int id)
    {
        var doctor = await _unitOfWork.Doctors.GetByIdAsync(id);

        if (doctor is null) return null;

        return MapToDto(doctor, doctor.Department?.Name ?? "N/A");
    }

    /// <summary>
    /// Step 5 - View 2: Doctor planning with department and upcoming consultations.
    /// </summary>
    public async Task<DoctorPlanningDto?> GetPlanningAsync(int id)
    {
        var doctor = await _unitOfWork.Doctors.GetByIdWithPlanningAsync(id);

        if (doctor is null) return null;

        return new DoctorPlanningDto
        {
            Id = doctor.Id,
            FullName = $"Dr. {doctor.FirstName} {doctor.LastName}",
            Specialty = doctor.Specialty.ToString(),
            DepartmentName = doctor.Department.Name,
            UpcomingConsultations = doctor.Consultations
                .Select(c => new ConsultationDto
                {
                    Id = c.Id,
                    Date = c.Date,
                    Status = c.Status.ToString(),
                    Notes = c.Notes,
                    Reason = c.Reason,
                    PatientId = c.PatientId,
                    PatientName = $"{c.Patient.FirstName} {c.Patient.LastName}",
                    DoctorId = doctor.Id,
                    DoctorName = $"Dr. {doctor.FirstName} {doctor.LastName}"
                }).ToList()
        };
    }

    public async Task<IEnumerable<DoctorDto>> GetAllAsync()
    {
        var doctors = await _unitOfWork.Doctors.GetAllAsync();

        return doctors.Select(d => MapToDto(d, d.Department?.Name ?? "N/A"));
    }

    public async Task<IEnumerable<DoctorDto>> GetByDepartmentAsync(int departmentId)
    {
        // Validate department exists
        _ = await _unitOfWork.Departments.GetByIdAsync(departmentId)
            ?? throw new KeyNotFoundException($"Department with ID {departmentId} not found.");

        var doctors = await _unitOfWork.Doctors.GetByDepartmentAsync(departmentId);

        return doctors.Select(d => MapToDto(d, d.Department?.Name ?? "N/A"));
    }

    public async Task DeleteAsync(int id)
    {
        var doctor = await _unitOfWork.Doctors.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Doctor with ID {id} not found.");

        try
        {
            _unitOfWork.Doctors.Delete(doctor);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new InvalidOperationException(
                $"Cannot delete Dr. {doctor.FirstName} {doctor.LastName}. " +
                "They may have existing consultations. Cancel or reassign them first.");
        }
    }

    private static DoctorDto MapToDto(Doctor doctor, string departmentName)
    {
        return new DoctorDto
        {
            Id = doctor.Id,
            FirstName = doctor.FirstName,
            LastName = doctor.LastName,
            LicenseNumber = doctor.LicenseNumber,
            Specialty = doctor.Specialty.ToString(),
            DepartmentName = departmentName,
            DepartmentId = doctor.DepartmentId
        };
    }
}