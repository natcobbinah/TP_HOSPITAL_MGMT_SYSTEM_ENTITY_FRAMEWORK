using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Enums;
using HospitalManagement.Domain.Interfaces;
using HospitalManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Repositories;

public class ConsultationRepository : IConsultationRepository
{
    private readonly HospitalDbContext _context;

    public ConsultationRepository(HospitalDbContext context)
    {
        _context = context;
    }

    public async Task<Consultation?> GetByIdAsync(int id)
        => await _context.Consultations
            .Include(c => c.Patient)
            .Include(c => c.Doctor)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<IEnumerable<Consultation>> GetUpcomingByPatientAsync(int patientId, int page, int pageSize)
        => await _context.Consultations
            .AsNoTracking()
            .Where(c => c.PatientId == patientId
                && c.Date >= DateTime.UtcNow
                && c.Status != ConsultationStatus.Cancelled)
            .Include(c => c.Doctor)
            .OrderBy(c => c.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

    public async Task<IEnumerable<Consultation>> GetByDoctorAndDateAsync(int doctorId, DateTime date)
        => await _context.Consultations
            .AsNoTracking()
            .Where(c => c.DoctorId == doctorId && c.Date.Date == date.Date)
            .Include(c => c.Patient)
            .OrderBy(c => c.Date)
            .ToListAsync();

    public async Task<IEnumerable<Consultation>> GetTodayByDoctorAsync(int doctorId)
        => await GetByDoctorAndDateAsync(doctorId, DateTime.UtcNow);

    public async Task<bool> ExistsAsync(int patientId, int doctorId, DateTime date)
        => await _context.Consultations
            .AnyAsync(c => c.PatientId == patientId
                && c.DoctorId == doctorId
                && c.Date == date);

    public async Task AddAsync(Consultation consultation)
        => await _context.Consultations.AddAsync(consultation);

    public void Update(Consultation consultation)
        => _context.Consultations.Update(consultation);

    /// <summary>
    /// Step 5 - View 3: Count consultations per department (via doctor's department).
    /// Uses projection for performance — no entity materialization.
    /// </summary>
    public async Task<int> CountByDepartmentAsync(int departmentId)
        => await _context.Consultations
            .CountAsync(c => c.Doctor.DepartmentId == departmentId);
}