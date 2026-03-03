using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Domain.Entities;

/// <summary>
/// Represents a hospital department (e.g., Cardiology, Neurology).
/// Supports hierarchical sub-departments via ParentDepartmentId (Step 6 - Feature 3).
/// </summary>
public class Department
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Location { get; set; } = string.Empty;

    // Address as owned type (Step 6 - Feature 1: factorized address)
    public Address ContactAddress { get; set; } = new Address();

    /// <summary>
    /// Head of department — nullable because it can be unassigned initially.
    /// This creates a 1-to-1 optional relationship with Doctor.
    /// </summary>
    public int? HeadDoctorId { get; set; }
    public Doctor? HeadDoctor { get; set; }

    /// <summary>
    /// Self-referencing relationship for department hierarchy (Step 6 - Feature 3).
    /// </summary>
    public int? ParentDepartmentId { get; set; }
    public Department? ParentDepartment { get; set; }
    public ICollection<Department> SubDepartments { get; set; } = new List<Department>();

    // Navigation properties
    public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}