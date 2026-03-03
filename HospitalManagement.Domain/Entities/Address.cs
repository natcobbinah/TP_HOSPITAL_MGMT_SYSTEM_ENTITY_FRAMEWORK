using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Domain.Entities;

/// <summary>
/// Value Object representing a physical address.
/// Configured as an Owned Type in EF Core so it is stored in the parent table.
/// Reused across Patient and Department entities (Step 6 - Feature 1).
/// </summary>
public class Address
{
    [MaxLength(200)]
    public string Street { get; set; } = string.Empty;

    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [MaxLength(20)]
    public string ZipCode { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Country { get; set; } = string.Empty;
}