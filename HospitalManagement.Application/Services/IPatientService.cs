using HospitalManagement.Application.Common;
using HospitalManagement.Application.DTOs;

namespace HospitalManagement.Application.Services;

public interface IPatientService
{
    Task<PatientDto> CreateAsync(CreatePatientDto dto);
    Task<PatientDto> UpdateAsync(int id, UpdatePatientDto dto);
    Task DeleteAsync(int id);
    Task<PatientDto?> GetByIdAsync(int id);
    Task<PatientDetailDto?> GetDetailAsync(int id);
    Task<PagedResult<PatientDto>> GetAllAsync(int page = 1, int pageSize = 20);
    Task<PagedResult<PatientDto>> SearchByNameAsync(string name, int page = 1, int pageSize = 20);
}