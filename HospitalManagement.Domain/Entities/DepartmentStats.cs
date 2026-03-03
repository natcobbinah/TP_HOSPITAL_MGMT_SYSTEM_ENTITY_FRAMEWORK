namespace HospitalManagement.Domain.Entities;

/// <summary>
/// Lightweight projection model for department statistics.
/// </summary>
public class DepartmentStats
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int DoctorCount { get; set; }
    public int ConsultationCount { get; set; }
}