using HospitalManagement.Domain.Entities;

namespace HospitalManagement.Domain.Interfaces;

public interface IDepartmentRepository
{
    Task<Department?> GetByIdAsync(int id);
    Task<Department?> GetByIdWithDoctorsAsync(int id);
    Task<IEnumerable<Department>> GetAllAsync();
    Task<IEnumerable<DepartmentStats>> GetAllWithStatsAsync();
    Task AddAsync(Department department);
    void Update(Department department);
    void Delete(Department department);
}