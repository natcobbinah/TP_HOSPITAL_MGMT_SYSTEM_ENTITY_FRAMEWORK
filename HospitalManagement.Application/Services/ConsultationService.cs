using HospitalManagement.Application.Common;
using HospitalManagement.Application.DTOs;
using HospitalManagement.Domain.Entities;
using HospitalManagement.Domain.Enums;
using HospitalManagement.Domain.Interfaces;

namespace HospitalManagement.Application.Services;

public class ConsultationService : IConsultationService
{
    private readonly IUnitOfWork _unitOfWork;

    public ConsultationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Plans a new consultation.
    /// Validates:
    /// - Patient exists
    /// - Doctor exists
    /// - Consultation date is in the future
    /// - No duplicate (same patient + same doctor + same date/time)
    /// </summary>
    public async Task<ConsultationDto> PlanAsync(CreateConsultationDto dto)
    {
        // Validate consultation date is in the future
        if (dto.Date <= DateTime.UtcNow)
            throw new ArgumentException("Consultation date must be in the future.");

        // Validate patient exists
        var patient = await _unitOfWork.Patients.GetByIdAsync(dto.PatientId)
            ?? throw new KeyNotFoundException($"Patient with ID {dto.PatientId} not found.");

        // Validate doctor exists
        var doctor = await _unitOfWork.Doctors.GetByIdAsync(dto.DoctorId)
            ?? throw new KeyNotFoundException($"Doctor with ID {dto.DoctorId} not found.");

        // Check for duplicate: same patient, same doctor, same date/time
        if (await _unitOfWork.Consultations.ExistsAsync(dto.PatientId, dto.DoctorId, dto.Date))
            throw new InvalidOperationException(
                $"A consultation already exists for patient '{patient.FirstName} {patient.LastName}' " +
                $"with Dr. {doctor.FirstName} {doctor.LastName} at {dto.Date:yyyy-MM-dd HH:mm}.");

        var consultation = new Consultation
        {
            Date = dto.Date,
            Status = ConsultationStatus.Planned,
            Reason = dto.Reason,
            PatientId = dto.PatientId,
            DoctorId = dto.DoctorId
        };

        await _unitOfWork.Consultations.AddAsync(consultation);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(consultation, patient, doctor);
    }

    /// <summary>
    /// Updates the status of an existing consultation (Planned → Completed, or add notes).
    /// Validates status transitions:
    /// - Cancelled consultations cannot be modified
    /// - Only valid enum values accepted
    /// </summary>
    public async Task<ConsultationDto> UpdateStatusAsync(int id, UpdateConsultationStatusDto dto)
    {
        var consultation = await _unitOfWork.Consultations.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Consultation with ID {id} not found.");

        // Validate current status allows modification
        if (consultation.Status == ConsultationStatus.Cancelled)
            throw new InvalidOperationException("Cannot modify a cancelled consultation.");

        // Parse and validate the new status
        if (!Enum.TryParse<ConsultationStatus>(dto.Status, ignoreCase: true, out var newStatus))
            throw new ArgumentException(
                $"Invalid status '{dto.Status}'. Valid values: {string.Join(", ", Enum.GetNames<ConsultationStatus>())}");

        consultation.Status = newStatus;

        if (!string.IsNullOrWhiteSpace(dto.Notes))
            consultation.Notes = dto.Notes;

        _unitOfWork.Consultations.Update(consultation);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(consultation, consultation.Patient, consultation.Doctor);
    }

    /// <summary>
    /// Cancels a consultation by setting its status to Cancelled.
    /// Only Planned consultations can be cancelled.
    /// </summary>
    public async Task<ConsultationDto> CancelAsync(int id)
    {
        var consultation = await _unitOfWork.Consultations.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Consultation with ID {id} not found.");

        if (consultation.Status == ConsultationStatus.Cancelled)
            throw new InvalidOperationException("Consultation is already cancelled.");

        if (consultation.Status == ConsultationStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed consultation.");

        consultation.Status = ConsultationStatus.Cancelled;

        _unitOfWork.Consultations.Update(consultation);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(consultation, consultation.Patient, consultation.Doctor);
    }

    /// <summary>
    /// Gets a single consultation by ID with Patient and Doctor info.
    /// </summary>
    public async Task<ConsultationDto?> GetByIdAsync(int id)
    {
        var consultation = await _unitOfWork.Consultations.GetByIdAsync(id);

        if (consultation is null) return null;

        return MapToDto(consultation, consultation.Patient, consultation.Doctor);
    }

    /// <summary>
    /// Lists upcoming (future, non-cancelled) consultations for a patient, with pagination.
    /// </summary>
    public async Task<PagedResult<ConsultationDto>> GetUpcomingByPatientAsync(
        int patientId, int page = 1, int pageSize = 20)
    {
        // Validate patient exists
        var patient = await _unitOfWork.Patients.GetByIdAsync(patientId)
            ?? throw new KeyNotFoundException($"Patient with ID {patientId} not found.");

        var consultations = await _unitOfWork.Consultations
            .GetUpcomingByPatientAsync(patientId, page, pageSize);

        var items = consultations.Select(c =>
            MapToDto(c, patient, c.Doctor)).ToList();

        return new PagedResult<ConsultationDto>
        {
            Items = items,
            TotalCount = items.Count, // Simplified; ideally a separate count query
            Page = page,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Lists all consultations for a doctor today.
    /// Used for the doctor's daily schedule.
    /// </summary>
    public async Task<IEnumerable<ConsultationDto>> GetTodayByDoctorAsync(int doctorId)
    {
        // Validate doctor exists
        var doctor = await _unitOfWork.Doctors.GetByIdAsync(doctorId)
            ?? throw new KeyNotFoundException($"Doctor with ID {doctorId} not found.");

        var consultations = await _unitOfWork.Consultations.GetTodayByDoctorAsync(doctorId);

        return consultations.Select(c =>
            MapToDto(c, c.Patient, doctor));
    }

    // ──────────────────────────────────────────────
    // Private helper: Entity → DTO mapping
    // ──────────────────────────────────────────────

    private static ConsultationDto MapToDto(Consultation consultation, Patient patient, Doctor doctor)
    {
        return new ConsultationDto
        {
            Id = consultation.Id,
            Date = consultation.Date,
            Status = consultation.Status.ToString(),
            Notes = consultation.Notes,
            Reason = consultation.Reason,
            PatientId = consultation.PatientId,
            PatientName = $"{patient.FirstName} {patient.LastName}",
            DoctorId = consultation.DoctorId,
            DoctorName = $"Dr. {doctor.FirstName} {doctor.LastName}"
        };
    }
}