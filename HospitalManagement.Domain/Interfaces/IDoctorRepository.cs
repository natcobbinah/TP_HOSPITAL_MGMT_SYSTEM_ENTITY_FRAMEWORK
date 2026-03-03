using HospitalManagement.Domain.Entities;

namespace HospitalManagement.Domain.Interfaces;

public interface IDoctorRepository
{
    Task<Doctor?> GetByIdAsync(int id);
    Task<Doctor?> GetByIdWithPlanningAsync(int id);
    Task<IEnumerable<Doctor>> GetByDepartmentAsync(int departmentId);
    Task<IEnumerable<Doctor>> GetAllAsync();
    Task AddAsync(Doctor doctor);
    void Update(Doctor doctor);
    void Delete(Doctor doctor);
}