using HospitalManagement.Application.DTOs;
using HospitalManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConsultationsController : ControllerBase
{
    private readonly IConsultationService _consultationService;

    public ConsultationsController(IConsultationService consultationService)
    {
        _consultationService = consultationService;
    }

    /// <summary>GET /api/consultations/5</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var consultation = await _consultationService.GetByIdAsync(id);
        return consultation is null ? NotFound() : Ok(consultation);
    }

    /// <summary>GET /api/consultations/patient/5/upcoming?page=1&pageSize=20</summary>
    [HttpGet("patient/{patientId:int}/upcoming")]
    public async Task<IActionResult> GetUpcomingByPatient(
        int patientId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var result = await _consultationService.GetUpcomingByPatientAsync(patientId, page, pageSize);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>GET /api/consultations/doctor/3/today</summary>
    [HttpGet("doctor/{doctorId:int}/today")]
    public async Task<IActionResult> GetTodayByDoctor(int doctorId)
    {
        try
        {
            var consultations = await _consultationService.GetTodayByDoctorAsync(doctorId);
            return Ok(consultations);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>POST /api/consultations</summary>
    [HttpPost]
    public async Task<IActionResult> Plan([FromBody] CreateConsultationDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var consultation = await _consultationService.PlanAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = consultation.Id }, consultation);
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

    /// <summary>PUT /api/consultations/5/status</summary>
    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateConsultationStatusDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var consultation = await _consultationService.UpdateStatusAsync(id, dto);
            return Ok(consultation);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
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

    /// <summary>PUT /api/consultations/5/cancel</summary>
    [HttpPut("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            var consultation = await _consultationService.CancelAsync(id);
            return Ok(consultation);
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