using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Interfaces;
using HospitalManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly HospitalDbContext _context;

    public PatientRepository(HospitalDbContext context)
    {
        _context = context;
    }

    public async Task<Patient?> GetByIdAsync(int id)
        => await _context.Patients.FindAsync(id);

    public async Task<Patient?> GetByFileNumberAsync(string fileNumber)
        => await _context.Patients
            .AsNoTracking() // Read-only optimization
            .FirstOrDefaultAsync(p => p.FileNumber == fileNumber);

    /// <summary>
    /// Eager loading: Include consultations + doctors to avoid N+1 problem.
    /// Used for the patient detail view (Step 5 - View 1).
    /// </summary>
    public async Task<Patient?> GetByIdWithConsultationsAsync(int id)
        => await _context.Patients
            .Include(p => p.Consultations)
                .ThenInclude(c => c.Doctor)
            .FirstOrDefaultAsync(p => p.Id == id);

    /// <summary>
    /// Paginated listing — avoids loading all patients at once.
    /// Ordered alphabetically by last name as required.
    /// AsNoTracking for read-only performance.
    /// </summary>
    public async Task<IEnumerable<Patient>> GetAllAsync(int page, int pageSize)
        => await _context.Patients
            .AsNoTracking()
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

    public async Task<IEnumerable<Patient>> SearchByNameAsync(string name, int page, int pageSize)
        => await _context.Patients
            .AsNoTracking()
            .Where(p => p.LastName.Contains(name) || p.FirstName.Contains(name))
            .OrderBy(p => p.LastName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

    public async Task<int> GetTotalCountAsync()
        => await _context.Patients.CountAsync();

    public async Task<int> GetSearchCountAsync(string name)
        => await _context.Patients
            .CountAsync(p => p.LastName.Contains(name) || p.FirstName.Contains(name));

    public async Task AddAsync(Patient patient)
        => await _context.Patients.AddAsync(patient);

    public void Update(Patient patient)
        => _context.Patients.Update(patient);

    public void Delete(Patient patient)
        => _context.Patients.Remove(patient);

    public async Task<bool> EmailExistsAsync(string email, int? excludePatientId = null)
        => await _context.Patients
            .AnyAsync(p => p.Email == email && (!excludePatientId.HasValue || p.Id != excludePatientId.Value));

    public async Task<bool> FileNumberExistsAsync(string fileNumber, int? excludePatientId = null)
        => await _context.Patients
            .AnyAsync(p => p.FileNumber == fileNumber && (!excludePatientId.HasValue || p.Id != excludePatientId.Value));
}