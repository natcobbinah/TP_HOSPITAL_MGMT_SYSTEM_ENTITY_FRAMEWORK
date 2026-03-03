using HospitalManagement.Application.DTOs;
using HospitalManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HospitalManagement.API.Pages.Dashboard;

public class PatientRecordModel : PageModel
{
    private readonly IPatientService _patientService;

    public PatientRecordModel(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [BindProperty(SupportsGet = true)]
    public int? PatientId { get; set; }

    public PatientDetailDto? PatientDetail { get; set; }
    public List<SelectListItem> PatientList { get; set; } = new();

    public async Task OnGetAsync()
    {
        // Load patient dropdown
        var allPatients = await _patientService.GetAllAsync(1, 200);
        PatientList = allPatients.Items.Select(p => new SelectListItem
        {
            Value = p.Id.ToString(),
            Text = $"{p.LastName}, {p.FirstName} ({p.FileNumber})",
            Selected = p.Id == PatientId
        }).ToList();

        // Load patient detail if selected
        if (PatientId.HasValue)
        {
            PatientDetail = await _patientService.GetDetailAsync(PatientId.Value);
        }
    }
}