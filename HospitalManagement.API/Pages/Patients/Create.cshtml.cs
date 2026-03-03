using HospitalManagement.Application.DTOs;
using HospitalManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospitalManagement.API.Pages.Patients;

public class CreateModel : PageModel
{
    private readonly IPatientService _patientService;

    public CreateModel(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [BindProperty]
    public CreatePatientDto Patient { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        try
        {
            await _patientService.CreateAsync(Patient);
            TempData["Success"] = "Patient created successfully.";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
    }
}