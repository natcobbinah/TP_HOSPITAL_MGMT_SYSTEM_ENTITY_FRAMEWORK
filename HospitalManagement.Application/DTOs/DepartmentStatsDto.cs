namespace HospitalManagement.Application.DTOs;

/// <summary>
/// Step 5 - View 3: Department statistics.
/// </summary>
public class DepartmentStatsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int DoctorCount { get; set; }
    public int ConsultationCount { get; set; }
}