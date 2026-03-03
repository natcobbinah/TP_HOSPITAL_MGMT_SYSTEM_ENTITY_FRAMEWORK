using HospitalManagement.Application.DTOs;
using HospitalManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorsController : ControllerBase
{
    private readonly IDoctorService _doctorService;

    public DoctorsController(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    /// <summary>GET /api/doctors</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var doctors = await _doctorService.GetAllAsync();
        return Ok(doctors);
    }

    /// <summary>GET /api/doctors/5</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var doctor = await _doctorService.GetByIdAsync(id);
        return doctor is null ? NotFound() : Ok(doctor);
    }

    /// <summary>GET /api/doctors/5/planning — Step 5 View 2</summary>
    [HttpGet("{id:int}/planning")]
    public async Task<IActionResult> GetPlanning(int id)
    {
        var planning = await _doctorService.GetPlanningAsync(id);
        return planning is null ? NotFound() : Ok(planning);
    }

    /// <summary>GET /api/doctors/department/3</summary>
    [HttpGet("department/{departmentId:int}")]
    public async Task<IActionResult> GetByDepartment(int departmentId)
    {
        try
        {
            var doctors = await _doctorService.GetByDepartmentAsync(departmentId);
            return Ok(doctors);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>POST /api/doctors</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDoctorDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var doctor = await _doctorService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = doctor.Id }, doctor);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    /// <summary>DELETE /api/doctors/5</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _doctorService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }
}