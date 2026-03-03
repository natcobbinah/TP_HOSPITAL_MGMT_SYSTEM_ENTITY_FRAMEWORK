using HospitalManagement.Domain.Entities;

namespace HospitalManagement.Domain.Interfaces;

public interface IPatientRepository
{
    Task<Patient?> GetByIdAsync(int id);
    Task<Patient?> GetByFileNumberAsync(string fileNumber);
    Task<Patient?> GetByIdWithConsultationsAsync(int id);
    Task<IEnumerable<Patient>> GetAllAsync(int page, int pageSize);
    Task<IEnumerable<Patient>> SearchByNameAsync(string name, int page, int pageSize);
    Task<int> GetTotalCountAsync();
    Task<int> GetSearchCountAsync(string name);
    Task AddAsync(Patient patient);
    void Update(Patient patient);
    void Delete(Patient patient);
    Task<bool> EmailExistsAsync(string email, int? excludePatientId = null);
    Task<bool> FileNumberExistsAsync(string fileNumber, int? excludePatientId = null);
}