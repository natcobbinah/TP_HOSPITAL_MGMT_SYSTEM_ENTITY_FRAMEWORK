using HospitalManagement.Application.DTOs;
using HospitalManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HospitalManagement.API.Pages.Dashboard;

public class DoctorPlanningModel : PageModel
{
    private readonly IDoctorService _doctorService;

    public DoctorPlanningModel(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    [BindProperty(SupportsGet = true)]
    public int? DoctorId { get; set; }

    public DoctorPlanningDto? DoctorPlanning { get; set; }
    public List<SelectListItem> DoctorList { get; set; } = new();

    public async Task OnGetAsync()
    {
        // Load doctor dropdown
        var allDoctors = await _doctorService.GetAllAsync();
        DoctorList = allDoctors.Select(d => new SelectListItem
        {
            Value = d.Id.ToString(),
            Text = $"Dr. {d.LastName}, {d.FirstName} — {d.Specialty} ({d.DepartmentName})",
            Selected = d.Id == DoctorId
        }).ToList();

        // Load planning if a doctor is selected
        if (DoctorId.HasValue)
        {
            DoctorPlanning = await _doctorService.GetPlanningAsync(DoctorId.Value);
        }
    }
}