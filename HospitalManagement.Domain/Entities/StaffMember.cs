using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Domain.Entities;

/// <summary>
/// Base class for all hospital staff — uses TPH (Table-Per-Hierarchy) inheritance.
/// TPH chosen because: fewer tables, simpler queries, good for EF Core performance.
/// All staff types stored in a single "StaffMembers" table with a discriminator column.
/// </summary>
public abstract class StaffMember
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    public DateTime HireDate { get; set; }

    public decimal Salary { get; set; }
}