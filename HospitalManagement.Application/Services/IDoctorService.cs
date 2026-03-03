using HospitalManagement.Application.DTOs;

namespace HospitalManagement.Application.Services;

public interface IDoctorService
{
    Task<DoctorDto> CreateAsync(CreateDoctorDto dto);
    Task<DoctorDto?> GetByIdAsync(int id);
    Task<DoctorPlanningDto?> GetPlanningAsync(int id);
    Task<IEnumerable<DoctorDto>> GetAllAsync();
    Task<IEnumerable<DoctorDto>> GetByDepartmentAsync(int departmentId);
    Task DeleteAsync(int id);
}