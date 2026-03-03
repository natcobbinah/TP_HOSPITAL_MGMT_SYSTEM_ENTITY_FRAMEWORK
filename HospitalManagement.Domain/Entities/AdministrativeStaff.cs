using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Domain.Entities;

public class AdministrativeStaff : StaffMember
{
    [MaxLength(100)]
    public string Function { get; set; } = string.Empty;
}