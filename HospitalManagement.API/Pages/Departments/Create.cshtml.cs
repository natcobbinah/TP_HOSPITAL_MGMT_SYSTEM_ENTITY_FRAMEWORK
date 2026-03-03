using HospitalManagement.Application.DTOs;
using HospitalManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HospitalManagement.API.Pages.Departments;

public class CreateModel : PageModel
{
    private readonly IDepartmentService _departmentService;
    private readonly IDoctorService _doctorService;

    public CreateModel(IDepartmentService departmentService, IDoctorService doctorService)
    {
        _departmentService = departmentService;
        _doctorService = doctorService;
    }

    [BindProperty]
    public CreateDepartmentDto Department { get; set; } = new();

    public List<SelectListItem> DoctorList { get; set; } = new();
    public List<SelectListItem> ParentDepartmentList { get; set; } = new();

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
            await _departmentService.CreateAsync(Department);
            TempData["Success"] = "Department created successfully.";
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
        var doctors = await _doctorService.GetAllAsync();
        DoctorList = doctors.Select(d => new SelectListItem
        {
            Value = d.Id.ToString(),
            Text = $"Dr. {d.LastName}, {d.FirstName}"
        }).ToList();

        var departments = await _departmentService.GetAllAsync();
        ParentDepartmentList = departments.Select(d => new SelectListItem
        {
            Value = d.Id.ToString(),
            Text = d.Name
        }).ToList();
    }
}