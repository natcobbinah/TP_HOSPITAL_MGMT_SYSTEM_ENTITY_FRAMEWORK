using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Domain.Entities;

public class Nurse : StaffMember
{
    [MaxLength(100)]
    public string Service { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Grade { get; set; } = string.Empty;
}