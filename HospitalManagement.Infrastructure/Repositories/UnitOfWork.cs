using HospitalManagement.Domain.Interfaces;
using HospitalManagement.Infrastructure.Data;

namespace HospitalManagement.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly HospitalDbContext _context;
    private IPatientRepository? _patients;
    private IDepartmentRepository? _departments;
    private IDoctorRepository? _doctors;
    private IConsultationRepository? _consultations;

    public UnitOfWork(HospitalDbContext context)
    {
        _context = context;
    }

    public IPatientRepository Patients
        => _patients ??= new PatientRepository(_context);

    public IDepartmentRepository Departments
        => _departments ??= new DepartmentRepository(_context);

    public IDoctorRepository Doctors
        => _doctors ??= new DoctorRepository(_context);

    public IConsultationRepository Consultations
        => _consultations ??= new ConsultationRepository(_context);

    public async Task<int> SaveChangesAsync()
        => await _context.SaveChangesAsync();

    public void Dispose()
        => _context.Dispose();
}