using System.ComponentModel.DataAnnotations;
using HospitalManagement.Domain.Enums;

namespace HospitalManagement.Domain.Entities;

/// <summary>
/// Represents a medical consultation — the association between a Patient and a Doctor.
/// This is a Many-to-Many relationship with payload (date, status, notes).
/// 
/// Unique constraint on (PatientId, DoctorId, Date) prevents a patient from having
/// two consultations with the same doctor at the exact same time.
/// </summary>
public class Consultation
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public ConsultationStatus Status { get; set; } = ConsultationStatus.Planned;

    [MaxLength(2000)]
    public string Notes { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;

    // Foreign keys
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;
}