namespace HospitalManagement.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IPatientRepository Patients { get; }
    IDepartmentRepository Departments { get; }
    IDoctorRepository Doctors { get; }
    IConsultationRepository Consultations { get; }
    Task<int> SaveChangesAsync();
}