using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Application.DTOs;

public class ConsultationDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
}

public class CreateConsultationDto
{
    [Required]
    public DateTime Date { get; set; }

    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;

    [Required]
    public int PatientId { get; set; }

    [Required]
    public int DoctorId { get; set; }
}

public class UpdateConsultationStatusDto
{
    [Required]
    public string Status { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Notes { get; set; } = string.Empty;
}