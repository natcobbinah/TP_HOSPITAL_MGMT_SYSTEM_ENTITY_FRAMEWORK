using HospitalManagement.Domain.Entities;

namespace HospitalManagement.Domain.Interfaces;

public interface IConsultationRepository
{
    Task<Consultation?> GetByIdAsync(int id);
    Task<IEnumerable<Consultation>> GetUpcomingByPatientAsync(int patientId, int page, int pageSize);
    Task<IEnumerable<Consultation>> GetByDoctorAndDateAsync(int doctorId, DateTime date);
    Task<IEnumerable<Consultation>> GetTodayByDoctorAsync(int doctorId);
    Task<bool> ExistsAsync(int patientId, int doctorId, DateTime date);
    Task AddAsync(Consultation consultation);
    void Update(Consultation consultation);
    Task<int> CountByDepartmentAsync(int departmentId);
}