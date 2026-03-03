using HospitalManagement.Application.DTOs;
using HospitalManagement.Application.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospitalManagement.API.Pages.Dashboard;

public class DepartmentStatsModel : PageModel
{
    private readonly IDepartmentService _departmentService;

    public DepartmentStatsModel(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    public IEnumerable<DepartmentStatsDto> Stats { get; set; } = Enumerable.Empty<DepartmentStatsDto>();
    public int TotalDoctors { get; set; }
    public int TotalConsultations { get; set; }

    public async Task OnGetAsync()
    {
        Stats = await _departmentService.GetStatsAsync();
        TotalDoctors = Stats.Sum(s => s.DoctorCount);
        TotalConsultations = Stats.Sum(s => s.ConsultationCount);
    }
}