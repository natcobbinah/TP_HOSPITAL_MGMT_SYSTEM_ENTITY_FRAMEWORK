namespace HospitalManagement.Application.DTOs;

/// <summary>
/// Step 5 - View 2: Doctor planning with department info and upcoming consultations.
/// </summary>
public class DoctorPlanningDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public List<ConsultationDto> UpcomingConsultations { get; set; } = new();
}