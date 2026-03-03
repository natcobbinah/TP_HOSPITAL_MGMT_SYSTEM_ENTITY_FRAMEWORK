using HospitalManagement.Application.DTOs;
using HospitalManagement.Domain.Interfaces;

namespace HospitalManagement.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;

    public DashboardService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// VIEW 1: Patient Detail (Fiche patient)
    /// 
    /// Strategy: EAGER LOADING via Include/ThenInclude
    /// - Loads Patient + Consultations + Doctor names in one round-trip
    /// - Maps to DTO to avoid exposing navigation properties to the API layer
    /// - Consultations sorted by date descending (most recent first)
    /// </summary>
    public async Task<PatientDetailDto?> GetPatientDetailAsync(int patientId)
    {
        var patient = await _unitOfWork.Patients.GetByIdWithConsultationsAsync(patientId);

        if (patient is null) return null;

        return new PatientDetailDto
        {
            Id = patient.Id,
            FileNumber = patient.FileNumber,
            FullName = $"{patient.FirstName} {patient.LastName}",
            DateOfBirth = patient.DateOfBirth,
            Email = patient.Email,
            Phone = patient.Phone,
            Consultations = patient.Consultations.Select(c => new ConsultationDto
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
    /// VIEW 2: Doctor Planning (Planning médecin)
    /// 
    /// Strategy: EAGER LOADING with FILTERED INCLUDE
    /// - Only loads future, non-cancelled consultations (filtered at DB level)
    /// - Includes Department info and Patient names
    /// - More efficient than loading all consultations then filtering in C#
    /// </summary>
    public async Task<DoctorPlanningDto?> GetDoctorPlanningAsync(int doctorId)
    {
        var doctor = await _unitOfWork.Doctors.GetByIdWithPlanningAsync(doctorId);

        if (doctor is null) return null;

        return new DoctorPlanningDto
        {
            Id = doctor.Id,
            FullName = $"Dr. {doctor.FirstName} {doctor.LastName}",
            Specialty = doctor.Specialty.ToString(),
            DepartmentName = doctor.Department.Name,
            UpcomingConsultations = doctor.Consultations.Select(c => new ConsultationDto
            {
                Id = c.Id,
                Date = c.Date,
                Status = c.Status.ToString(),
                Notes = c.Notes,
                Reason = c.Reason,
                PatientId = c.PatientId,
                PatientName = $"{c.Patient.FirstName} {c.Patient.LastName}",
                DoctorId = c.DoctorId,
                DoctorName = $"Dr. {doctor.FirstName} {doctor.LastName}"
            }).ToList()
        };
    }

    /// <summary>
    /// VIEW 3: Department Statistics (Statistiques département)
    /// 
    /// Strategy: PROJECTION via Select()
    /// - No entity materialization — EF Core generates optimized SQL with COUNT
    /// - AsNoTracking is implicit with projections
    /// - Single database round-trip for all departments
    /// </summary>
    public async Task<IEnumerable<DepartmentStatsDto>> GetDepartmentStatsAsync()
    {
        var stats = await _unitOfWork.Departments.GetAllWithStatsAsync();

        return stats.Select(s => new DepartmentStatsDto
        {
            Id = s.Id,
            Name = s.Name,
            Location = s.Location,
            DoctorCount = s.DoctorCount,
            ConsultationCount = s.ConsultationCount
        });
    }
}