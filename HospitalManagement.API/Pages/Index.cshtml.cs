using HospitalManagement.Application.DTOs;
using HospitalManagement.Application.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospitalManagement.API.Pages;

public class IndexModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly IDoctorService _doctorService;
    private readonly IDepartmentService _departmentService;

    public IndexModel(
        IPatientService patientService,
        IDoctorService doctorService,
        IDepartmentService departmentService)
    {
        _patientService = patientService;
        _doctorService = doctorService;
        _departmentService = departmentService;
    }

    public int TotalPatients { get; set; }
    public int TotalDoctors { get; set; }
    public int TotalDepartments { get; set; }
    public IEnumerable<DepartmentStatsDto> DepartmentStats { get; set; } = Enumerable.Empty<DepartmentStatsDto>();

    public async Task OnGetAsync()
    {
        var patients = await _patientService.GetAllAsync(1, 1);
        TotalPatients = patients.TotalCount;

        var doctors = await _doctorService.GetAllAsync();
        TotalDoctors = doctors.Count();

        var departments = await _departmentService.GetAllAsync();
        TotalDepartments = departments.Count();

        DepartmentStats = await _departmentService.GetStatsAsync();
    }
}