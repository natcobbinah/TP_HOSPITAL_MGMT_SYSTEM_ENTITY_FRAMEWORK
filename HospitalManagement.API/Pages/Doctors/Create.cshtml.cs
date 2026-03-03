using HospitalManagement.Application.DTOs;
using HospitalManagement.Application.Services;
using HospitalManagement.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HospitalManagement.API.Pages.Doctors;

public class CreateModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly IDepartmentService _departmentService;

    public CreateModel(IDoctorService doctorService, IDepartmentService departmentService)
    {
        _doctorService = doctorService;
        _departmentService = departmentService;
    }

    [BindProperty]
    public CreateDoctorDto Doctor { get; set; } = new();

    public List<SelectListItem> DepartmentList { get; set; } = new();
    public List<SelectListItem> SpecialtyList { get; set; } = new();

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
            await _doctorService.CreateAsync(Doctor);
            TempData["Success"] = "Doctor added successfully.";
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
        var departments = await _departmentService.GetAllAsync();
        DepartmentList = departments.Select(d => new SelectListItem
        {
            Value = d.Id.ToString(),
            Text = d.Name
        }).ToList();

        SpecialtyList = Enum.GetNames<Specialty>().Select(s => new SelectListItem
        {
            Value = s,
            Text = s
        }).ToList();
    }
}