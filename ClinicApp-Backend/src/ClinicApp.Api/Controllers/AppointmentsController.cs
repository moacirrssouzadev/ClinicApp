using ClinicApp.Application.Dtos;
using ClinicApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApp.Api.Controllers;

/// <summary>
/// Controller responsável por operações com Agendamentos
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _service;
    private readonly ILogger<AppointmentsController> _logger;

    public AppointmentsController(IAppointmentService service, ILogger<AppointmentsController> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Agendar consulta
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AppointmentDto>> Create([FromBody] CreateAppointmentDto dto)
    {
        _logger.LogInformation("Agendando consulta para paciente: {PatientId} com profissional: {ProfessionalId}", 
            dto.PatientId, dto.HealthProfessionalId);

        var result = await _service.CreateAsync(dto);

        if (!result.IsSuccess)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
    }

    /// <summary>
    /// Obter agendamento por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AppointmentDto>> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result.Data);
    }

    /// <summary>
    /// Obter todos os agendamentos
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result.Data);
    }

    /// <summary>
    /// Obter agendamentos de um paciente
    /// </summary>
    [HttpGet("patient/{patientId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetByPatientId(Guid patientId)
    {
        var result = await _service.GetByPatientIdAsync(patientId);
        return Ok(result.Data);
    }

    /// <summary>
    /// Obter agendamentos de um profissional
    /// </summary>
    [HttpGet("professional/{healthProfessionalId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetByHealthProfessionalId(Guid healthProfessionalId)
    {
        var result = await _service.GetByHealthProfessionalIdAsync(healthProfessionalId);
        return Ok(result.Data);
    }

    /// <summary>
    /// Obter agenda de um profissional em uma determinada data
    /// </summary>
    [HttpGet("professional/{healthProfessionalId:guid}/schedule")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProfessionalScheduleDto>> GetProfessionalSchedule(
        Guid healthProfessionalId, 
        [FromQuery] DateTime date)
    {
        var result = await _service.GetProfessionalScheduleAsync(healthProfessionalId, date);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result.Data);
    }

    /// <summary>
    /// Obter horários disponíveis de um profissional em uma data
    /// </summary>
    [HttpGet("professional/{healthProfessionalId:guid}/available-slots")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AvailableTimeSlotDto>>> GetAvailableTimeSlots(
        Guid healthProfessionalId, 
        [FromQuery] DateTime date)
    {
        var result = await _service.GetAvailableTimeSlotsAsync(healthProfessionalId, date);
        return Ok(result.Data);
    }

    /// <summary>
    /// Completar agendamento
    /// </summary>
    [HttpPost("{id:guid}/complete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Complete(Guid id, [FromBody] CompleteAppointmentRequest? request = null)
    {
        _logger.LogInformation("Completando agendamento: {Id}", id);

        var result = await _service.CompleteAsync(id, request?.Notes);

        if (!result.IsSuccess)
            return result.Message.Contains("não encontrado") ? NotFound(result) : BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Cancelar agendamento
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelAppointmentRequest? request = null)
    {
        _logger.LogInformation("Cancelando agendamento: {Id}", id);

        var result = await _service.CancelAsync(id, request?.Notes);

        if (!result.IsSuccess)
            return result.Message.Contains("não encontrado") ? NotFound(result) : BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Marcar como não apresentado
    /// </summary>
    [HttpPost("{id:guid}/no-show")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsNoShow(Guid id, [FromBody] NoShowRequest? request = null)
    {
        _logger.LogInformation("Marcando como não apresentado: {Id}", id);

        var result = await _service.MarkAsNoShowAsync(id, request?.Notes);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Deletar agendamento
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        _logger.LogInformation("Deletando agendamento: {Id}", id);

        var result = await _service.DeleteAsync(id);

        if (!result.IsSuccess)
            return NotFound(result);

        return NoContent();
    }
}

// Dtos para requisições
public record CompleteAppointmentRequest(string? Notes);
public record CancelAppointmentRequest(string? Notes);
public record NoShowRequest(string? Notes);
