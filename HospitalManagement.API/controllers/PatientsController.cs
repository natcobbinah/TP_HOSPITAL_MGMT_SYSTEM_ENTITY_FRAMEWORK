using HospitalManagement.Application.DTOs;
using HospitalManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    /// <summary>GET /api/patients?page=1&pageSize=20</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _patientService.GetAllAsync(page, pageSize);
        return Ok(result);
    }

    /// <summary>GET /api/patients/5</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var patient = await _patientService.GetByIdAsync(id);
        return patient is null ? NotFound() : Ok(patient);
    }

    /// <summary>GET /api/patients/5/detail — Full patient file with consultations (Step 5)</summary>
    [HttpGet("{id:int}/detail")]
    public async Task<IActionResult> GetDetail(int id)
    {
        var detail = await _patientService.GetDetailAsync(id);
        return detail is null ? NotFound() : Ok(detail);
    }

    /// <summary>GET /api/patients/search?name=Dupont&page=1&pageSize=20</summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string name, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _patientService.SearchByNameAsync(name, page, pageSize);
        return Ok(result);
    }

    /// <summary>POST /api/patients</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePatientDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var patient = await _patientService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
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

    /// <summary>PUT /api/patients/5</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePatientDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var patient = await _patientService.UpdateAsync(id, dto);
            return Ok(patient);
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

    /// <summary>DELETE /api/patients/5</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _patientService.DeleteAsync(id);
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