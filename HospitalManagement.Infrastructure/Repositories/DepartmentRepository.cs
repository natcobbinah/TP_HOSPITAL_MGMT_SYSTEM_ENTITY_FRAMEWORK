using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Interfaces;
using HospitalManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly HospitalDbContext _context;

    public DepartmentRepository(HospitalDbContext context)
    {
        _context = context;
    }

    public async Task<Department?> GetByIdAsync(int id)
        => await _context.Departments.FindAsync(id);

    public async Task<Department?> GetByIdWithDoctorsAsync(int id)
        => await _context.Departments
            .Include(d => d.Doctors)
            .Include(d => d.HeadDoctor)
            .Include(d => d.SubDepartments)
            .FirstOrDefaultAsync(d => d.Id == id);

    public async Task<IEnumerable<Department>> GetAllAsync()
        => await _context.Departments
            .AsNoTracking()
            .Include(d => d.HeadDoctor)
            .OrderBy(d => d.Name)
            .ToListAsync();

    public async Task AddAsync(Department department)
        => await _context.Departments.AddAsync(department);

    public void Update(Department department)
        => _context.Departments.Update(department);

    public void Delete(Department department)
        => _context.Departments.Remove(department);

    public async Task<IEnumerable<DepartmentStats>> GetAllWithStatsAsync()
        => await _context.Departments
            .AsNoTracking()
            .OrderBy(d => d.Name)
            .Select(d => new DepartmentStats
            {
                Id = d.Id,
                Name = d.Name,
                Location = d.Location,
                DoctorCount = d.Doctors.Count,
                ConsultationCount = d.Doctors.SelectMany(doc => doc.Consultations).Count()
            })
            .ToListAsync();
}