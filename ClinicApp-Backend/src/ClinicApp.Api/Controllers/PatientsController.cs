using ClinicApp.Application.Dtos;
using ClinicApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApp.Api.Controllers;

/// <summary>
/// Controller responsável por operações com Pacientes
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _service;
    private readonly ILogger<PatientsController> _logger;

    public PatientsController(IPatientService service, ILogger<PatientsController> logger)
    {
        _service = service ?? 
            throw new ArgumentNullException(nameof(service));
        _logger = logger ?? 
            throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Criar novo paciente
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PatientDto>> Create([FromBody] CreatePatientDto dto)
    {
        _logger.LogInformation("Criando novo paciente: {Email}", dto.Email);

        var result = await _service.CreateAsync(dto);

        if (!result.IsSuccess)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
    }

    /// <summary>
    /// Obter paciente por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PatientDto>> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);

        if (!result.IsSuccess)
            return result.Message.Contains("não encontrado") ? 
                NotFound(result) : 
                StatusCode(StatusCodes.Status500InternalServerError, result);

        return Ok(result.Data);
    }

    /// <summary>
    /// Obter paciente por CPF
    /// </summary>
    [HttpGet("cpf/{cpf}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PatientDto>> GetByCpf(string cpf)
    {
        var result = await _service.GetByCpfAsync(cpf);

        if (!result.IsSuccess)
            return result.Message.Contains("não encontrado") ? 
                NotFound(result) : 
                StatusCode(StatusCodes.Status500InternalServerError, result);

        return Ok(result.Data);
    }

    /// <summary>
    /// Obter paciente por Email
    /// </summary>
    [HttpGet("email/{email}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PatientDto>> GetByEmail(string email)
    {
        var result = await _service.GetByEmailAsync(email);

        if (!result.IsSuccess)
            return result.Message.Contains("não encontrado") ? 
                NotFound(result) : 
                StatusCode(StatusCodes.Status500InternalServerError, result);

        return Ok(result.Data);
    }

    /// <summary>
    /// Listar todos os pacientes
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<PatientDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();

        if (!result.IsSuccess)
            return StatusCode(StatusCodes.Status500InternalServerError, result);

        return Ok(result.Data);
    }

    /// <summary>
    /// Listar pacientes ativos
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<PatientDto>>> GetActive()
    {
        var result = await _service.GetActiveAsync();

        if (!result.IsSuccess)
            return StatusCode(StatusCodes.Status500InternalServerError, result);

        return Ok(result.Data);
    }

    /// <summary>
    /// Atualizar paciente
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PatientDto>> Update(Guid id, [FromBody] UpdatePatientDto dto)
    {
        _logger.LogInformation("Atualizando paciente: {Id}", id);

        var result = await _service.UpdateAsync(id, dto);

        if (!result.IsSuccess)
            return result.Message.Contains("não encontrado") ? NotFound(result) : BadRequest(result);

        return Ok(result.Data);
    }

    /// <summary>
    /// Desativar paciente
    /// </summary>
    [HttpPost("{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        _logger.LogInformation("Desativando paciente: {Id}", id);

        var result = await _service.DeactivateAsync(id);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Ativar paciente
    /// </summary>
    [HttpPost("{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Activate(Guid id)
    {
        _logger.LogInformation("Ativando paciente: {Id}", id);

        var result = await _service.ActivateAsync(id);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Deletar paciente
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        _logger.LogInformation("Deletando paciente: {Id}", id);

        var result = await _service.DeleteAsync(id);

        if (!result.IsSuccess)
            return NotFound(result);

        return NoContent();
    }
}
