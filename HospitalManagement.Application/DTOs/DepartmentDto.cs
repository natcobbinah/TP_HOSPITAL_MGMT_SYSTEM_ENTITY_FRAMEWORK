using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Application.DTOs;

public class DepartmentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? HeadDoctorName { get; set; }
    public int? ParentDepartmentId { get; set; }
}

public class CreateDepartmentDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Location { get; set; } = string.Empty;

    public int? HeadDoctorId { get; set; }
    public int? ParentDepartmentId { get; set; }
}