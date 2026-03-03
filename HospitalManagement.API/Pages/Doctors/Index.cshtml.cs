using HospitalManagement.Application.DTOs;
using HospitalManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospitalManagement.API.Pages.Doctors;

public class IndexModel : PageModel
{
    private readonly IDoctorService _doctorService;

    public IndexModel(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    public IEnumerable<DoctorDto> Doctors { get; set; } = Enumerable.Empty<DoctorDto>();

    public async Task OnGetAsync()
    {
        Doctors = await _doctorService.GetAllAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        try
        {
            await _doctorService.DeleteAsync(id);
            TempData["Success"] = "Doctor deleted successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToPage();
    }
}