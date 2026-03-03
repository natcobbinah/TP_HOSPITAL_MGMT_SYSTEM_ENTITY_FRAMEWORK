using HospitalManagement.Application.DTOs;
using HospitalManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HospitalManagement.API.Pages.Consultations;

public class CreateModel : PageModel
{
    private readonly IConsultationService _consultationService;
    private readonly IPatientService _patientService;
    private readonly IDoctorService _doctorService;

    public CreateModel(
        IConsultationService consultationService,
        IPatientService patientService,
        IDoctorService doctorService)
    {
        _consultationService = consultationService;
        _patientService = patientService;
        _doctorService = doctorService;
    }

    [BindProperty]
    public CreateConsultationDto Consultation { get; set; } = new()
    {
        Date = DateTime.Now.AddDays(1).Date.AddHours(9) // Default: tomorrow 9 AM
    };

    public List<SelectListItem> PatientList { get; set; } = new();
    public List<SelectListItem> DoctorList { get; set; } = new();

    public async Task OnGetAsync()
    {
        await LoadDropdowns();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadDropdowns();
            return Page();
        }

        try
        {
            await _consultationService.PlanAsync(Consultation);
            TempData["Success"] = "Consultation scheduled successfully.";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadDropdowns();
            return Page();
        }
    }

    private async Task LoadDropdowns()
    {
        var patients = await _patientService.GetAllAsync(1, 200);
        PatientList = patients.Items.Select(p => new SelectListItem
        {
            Value = p.Id.ToString(),
            Text = $"{p.LastName}, {p.FirstName} ({p.FileNumber})"
        }).ToList();

        var doctors = await _doctorService.GetAllAsync();
        DoctorList = doctors.Select(d => new SelectListItem
        {
            Value = d.Id.ToString(),
            Text = $"Dr. {d.LastName}, {d.FirstName} — {d.Specialty}"
        }).ToList();
    }
}