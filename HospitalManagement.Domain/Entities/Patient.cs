using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Domain.Entities;

/// <summary>
/// Represents a hospital patient.
/// 
/// Concurrency Strategy: Application-managed concurrency token.
/// We use a string (Guid) that gets updated on every save.
/// This works across ALL database providers (SQLite, SQL Server, PostgreSQL).
/// </summary>
public class Patient
{
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string FileNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public Address Address { get; set; } = new Address();

    /// <summary>
    /// Application-managed concurrency token.
    /// Updated automatically on every SaveChanges via DbContext override.
    /// When two users load the same patient and both try to save,
    /// the second save detects the token mismatch and throws DbUpdateConcurrencyException.
    /// </summary>
    [MaxLength(36)]
    public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

    // Navigation properties
    public ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();
}