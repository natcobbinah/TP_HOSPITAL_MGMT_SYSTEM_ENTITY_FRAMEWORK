namespace HospitalManagement.Application.DTOs;

/// <summary>
/// Step 5 - View 1: Patient detail with all consultations.
/// </summary>
public class PatientDetailDto
{
    public int Id { get; set; }
    public string FileNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public List<ConsultationDto> Consultations { get; set; } = new();
}