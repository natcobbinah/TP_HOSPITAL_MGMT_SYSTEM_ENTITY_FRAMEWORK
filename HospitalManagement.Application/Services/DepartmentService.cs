using HospitalManagement.Application.DTOs;
using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IUnitOfWork _unitOfWork;

    public DepartmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto)
    {
        // Validate parent department if specified
        if (dto.ParentDepartmentId.HasValue)
        {
            _ = await _unitOfWork.Departments.GetByIdAsync(dto.ParentDepartmentId.Value)
                ?? throw new KeyNotFoundException(
                    $"Parent department with ID {dto.ParentDepartmentId.Value} not found.");
        }

        // Validate head doctor if specified
        if (dto.HeadDoctorId.HasValue)
        {
            _ = await _unitOfWork.Doctors.GetByIdAsync(dto.HeadDoctorId.Value)
                ?? throw new KeyNotFoundException(
                    $"Doctor with ID {dto.HeadDoctorId.Value} not found.");
        }

        var department = new Department
        {
            Name = dto.Name,
            Location = dto.Location,
            HeadDoctorId = dto.HeadDoctorId,
            ParentDepartmentId = dto.ParentDepartmentId
        };

        await _unitOfWork.Departments.AddAsync(department);

        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") == true)
        {
            throw new InvalidOperationException($"Department name '{dto.Name}' already exists.");
        }

        return MapToDto(department);
    }

    public async Task<DepartmentDto?> GetByIdAsync(int id)
    {
        var department = await _unitOfWork.Departments.GetByIdAsync(id);
        return department is null ? null : MapToDto(department);
    }

    public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
    {
        var departments = await _unitOfWork.Departments.GetAllAsync();
        return departments.Select(MapToDto);
    }

    /// <summary>
    /// Step 5 - View 3: Department statistics using projections.
    /// </summary>
    public async Task<IEnumerable<DepartmentStatsDto>> GetStatsAsync()
    {
        var stats = await _unitOfWork.Departments.GetAllWithStatsAsync();

        return stats.Select(s => new DepartmentStatsDto
        {
            Id = s.Id,
            Name = s.Name,
            Location = s.Location,
            DoctorCount = s.DoctorCount,
            ConsultationCount = s.ConsultationCount
        });
    }

    public async Task DeleteAsync(int id)
    {
        var department = await _unitOfWork.Departments.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Department with ID {id} not found.");

        try
        {
            _unitOfWork.Departments.Delete(department);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new InvalidOperationException(
                $"Cannot delete department '{department.Name}'. " +
                "It still has doctors or sub-departments assigned. Reassign them first.");
        }
    }

    private static DepartmentDto MapToDto(Department department)
    {
        return new DepartmentDto
        {
            Id = department.Id,
            Name = department.Name,
            Location = department.Location,
            HeadDoctorName = department.HeadDoctor is not null
                ? $"Dr. {department.HeadDoctor.FirstName} {department.HeadDoctor.LastName}"
                : null,
            ParentDepartmentId = department.ParentDepartmentId
        };
    }
}