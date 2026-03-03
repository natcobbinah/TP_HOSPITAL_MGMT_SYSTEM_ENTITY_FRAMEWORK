using HospitalManagement.Application.DTOs;
using HospitalManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospitalManagement.API.Pages.Consultations;

public class IndexModel : PageModel
{
    private readonly IConsultationService _consultationService;
    private readonly IDoctorService _doctorService;

    public IndexModel(IConsultationService consultationService, IDoctorService doctorService)
    {
        _consultationService = consultationService;
        _doctorService = doctorService;
    }

    public IEnumerable<ConsultationDto> TodayConsultations { get; set; } = Enumerable.Empty<ConsultationDto>();
    public IEnumerable<DoctorDto> Doctors { get; set; } = Enumerable.Empty<DoctorDto>();

    [BindProperty(SupportsGet = true)]
    public int? DoctorId { get; set; }

    public async Task OnGetAsync()
    {
        Doctors = await _doctorService.GetAllAsync();

        if (DoctorId.HasValue)
        {
            try
            {
                TodayConsultations = await _consultationService.GetTodayByDoctorAsync(DoctorId.Value);
            }
            catch { /* Doctor not found — leave empty */ }
        }
    }

    public async Task<IActionResult> OnPostCancelAsync(int id)
    {
        try
        {
            await _consultationService.CancelAsync(id);
            TempData["Success"] = "Consultation cancelled.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToPage(new { DoctorId });
    }

    public async Task<IActionResult> OnPostCompleteAsync(int id)
    {
        try
        {
            await _consultationService.UpdateStatusAsync(id, new UpdateConsultationStatusDto
            {
                Status = "Completed",
                Notes = "Consultation completed."
            });
            TempData["Success"] = "Consultation marked as completed.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToPage(new { DoctorId });
    }
}