using HospitalManagement.Application.DTOs;
using HospitalManagement.Application.Common;
using HospitalManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospitalManagement.API.Pages.Patients;

public class IndexModel : PageModel
{
    private readonly IPatientService _patientService;

    public IndexModel(IPatientService patientService)
    {
        _patientService = patientService;
    }

    public PagedResult<PatientDto> Patients { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNum { get; set; } = 1;

    public async Task OnGetAsync()
    {
        Patients = string.IsNullOrWhiteSpace(Search)
            ? await _patientService.GetAllAsync(PageNum, 10)
            : await _patientService.SearchByNameAsync(Search, PageNum, 10);
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        try
        {
            await _patientService.DeleteAsync(id);
            TempData["Success"] = "Patient deleted successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToPage();
    }
}