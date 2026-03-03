using HospitalManagement.Application.Common;
using HospitalManagement.Application.DTOs;

namespace HospitalManagement.Application.Services;

public interface IConsultationService
{
    Task<ConsultationDto> PlanAsync(CreateConsultationDto dto);
    Task<ConsultationDto> UpdateStatusAsync(int id, UpdateConsultationStatusDto dto);
    Task<ConsultationDto> CancelAsync(int id);
    Task<ConsultationDto?> GetByIdAsync(int id);
    Task<PagedResult<ConsultationDto>> GetUpcomingByPatientAsync(int patientId, int page = 1, int pageSize = 20);
    Task<IEnumerable<ConsultationDto>> GetTodayByDoctorAsync(int doctorId);
}