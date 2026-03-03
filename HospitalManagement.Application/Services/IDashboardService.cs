using HospitalManagement.Application.DTOs;

namespace HospitalManagement.Application.Services;

/// <summary>
/// Service dedicated to Step 5 dashboard views.
/// Separated from CRUD services for single responsibility.
/// </summary>
public interface IDashboardService
{
    /// <summary>View 1: Patient file with all consultations</summary>
    Task<PatientDetailDto?> GetPatientDetailAsync(int patientId);

    /// <summary>View 2: Doctor planning with upcoming consultations</summary>
    Task<DoctorPlanningDto?> GetDoctorPlanningAsync(int doctorId);

    /// <summary>View 3: All department statistics</summary>
    Task<IEnumerable<DepartmentStatsDto>> GetDepartmentStatsAsync();
}