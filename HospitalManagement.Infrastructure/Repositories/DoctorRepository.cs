using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Interfaces;
using HospitalManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Repositories;

public class DoctorRepository : IDoctorRepository
{
    private readonly HospitalDbContext _context;

    public DoctorRepository(HospitalDbContext context)
    {
        _context = context;
    }

    public async Task<Doctor?> GetByIdAsync(int id)
        => await _context.Doctors
            .Include(d => d.Department)
            .FirstOrDefaultAsync(d => d.Id == id);

    /// <summary>
    /// Step 5 - View 2: Doctor planning with department and upcoming consultations.
    /// Eager loading to avoid N+1.
    /// </summary>
    public async Task<Doctor?> GetByIdWithPlanningAsync(int id)
        => await _context.Doctors
            .Include(d => d.Department)
            .Include(d => d.Consultations.Where(c => c.Date >= DateTime.UtcNow))
                .ThenInclude(c => c.Patient)
            .FirstOrDefaultAsync(d => d.Id == id);

    public async Task<IEnumerable<Doctor>> GetByDepartmentAsync(int departmentId)
        => await _context.Doctors
            .AsNoTracking()
            .Where(d => d.DepartmentId == departmentId)
            .OrderBy(d => d.LastName)
            .ToListAsync();

    public async Task<IEnumerable<Doctor>> GetAllAsync()
        => await _context.Doctors
            .AsNoTracking()
            .Include(d => d.Department)
            .OrderBy(d => d.LastName)
            .ToListAsync();

    public async Task AddAsync(Doctor doctor)
        => await _context.Doctors.AddAsync(doctor);

    public void Update(Doctor doctor)
        => _context.Doctors.Update(doctor);

    public void Delete(Doctor doctor)
        => _context.Doctors.Remove(doctor);
}