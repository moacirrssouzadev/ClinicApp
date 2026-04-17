using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClinicApp.Application.Dtos;
using ClinicApp.Application.Services;

namespace ClinicApp.Api.Controllers;

/// <summary>
/// Controller responsável por operações com Profissionais de Saúde
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HealthProfessionalsController : ControllerBase
{
    private readonly IHealthProfessionalService _service;
    private readonly ILogger<HealthProfessionalsController> _logger;

    public HealthProfessionalsController(IHealthProfessionalService service, ILogger<HealthProfessionalsController> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Criar novo profissional de saúde
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<HealthProfessionalDto>> Create([FromBody] CreateHealthProfessionalDto dto)
    {
        _logger.LogInformation("Criando novo profissional: {Email}", dto.Email);

        var result = await _service.CreateAsync(dto);

        if (!result.IsSuccess)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
    }

    /// <summary>
    /// Obter profissional por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HealthProfessionalDto>> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result.Data);
    }

    /// <summary>
    /// Obter profissional por CPF
    /// </summary>
    [HttpGet("cpf/{cpf}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HealthProfessionalDto>> GetByCpf(string cpf)
    {
        var result = await _service.GetByCpfAsync(cpf);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result.Data);
    }

    /// <summary>
    /// Listar todos os profissionais
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<HealthProfessionalDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result.Data);
    }

    /// <summary>
    /// Listar profissionais ativos
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<HealthProfessionalDto>>> GetActive()
    {
        var result = await _service.GetActiveAsync();
        return Ok(result.Data);
    }

    /// <summary>
    /// Listar profissionais por especialização
    /// </summary>
    [HttpGet("specialization/{specialization}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<HealthProfessionalDto>>> GetBySpecialization(string specialization)
    {
        var result = await _service.GetBySpecializationAsync(specialization);
        return Ok(result.Data);
    }

    /// <summary>
    /// Atualizar profissional
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HealthProfessionalDto>> Update(Guid id, [FromBody] UpdateHealthProfessionalDto dto)
    {
        _logger.LogInformation("Atualizando profissional: {Id}", id);

        var result = await _service.UpdateAsync(id, dto);

        if (!result.IsSuccess)
            return result.Message.Contains("não encontrado") ? NotFound(result) : BadRequest(result);

        return Ok(result.Data);
    }

    /// <summary>
    /// Desativar profissional
    /// </summary>
    [HttpPost("{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        _logger.LogInformation("Desativando profissional: {Id}", id);

        var result = await _service.DeactivateAsync(id);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Ativar profissional
    /// </summary>
    [HttpPost("{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Activate(Guid id)
    {
        _logger.LogInformation("Ativando profissional: {Id}", id);

        var result = await _service.ActivateAsync(id);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Deletar profissional
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        _logger.LogInformation("Deletando profissional: {Id}", id);

        var result = await _service.DeleteAsync(id);

        if (!result.IsSuccess)
            return NotFound(result);

        return NoContent();
    }
}
