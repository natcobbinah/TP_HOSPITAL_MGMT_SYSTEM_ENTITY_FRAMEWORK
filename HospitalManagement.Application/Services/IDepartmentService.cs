using HospitalManagement.Application.DTOs;

namespace HospitalManagement.Application.Services;

public interface IDepartmentService
{
    Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto);
    Task<DepartmentDto?> GetByIdAsync(int id);
    Task<IEnumerable<DepartmentDto>> GetAllAsync();
    Task<IEnumerable<DepartmentStatsDto>> GetStatsAsync();
    Task DeleteAsync(int id);
}