using System.ComponentModel.DataAnnotations;
using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Domain.Entities;

/// <summary>
/// Represents a medical doctor in the hospital.
/// Relationship with Department: Many-to-One (a doctor belongs to exactly one department).
/// DeleteBehavior: Restrict — cannot delete a department that still has doctors.
/// </summary>
public class Doctor
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Unique medical license number.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string LicenseNumber { get; set; } = string.Empty;

    public Specialty Specialty { get; set; }

    public DateTime HireDate { get; set; }

    // Foreign key to Department
    public int DepartmentId { get; set; }
    public Department Department { get; set; } = null!;

    // Navigation properties
    public ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();
}