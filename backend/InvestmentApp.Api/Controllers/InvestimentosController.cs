using InvestmentApp.Api.DTOs;
using InvestmentApp.Api.Models;
using InvestmentApp.Api.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace InvestmentApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvestimentosController : ControllerBase
{
    private readonly InvestimentoService _investimentoService;

    public InvestimentosController(InvestimentoService investimentoService)
    {
        _investimentoService = investimentoService;
    }

    [HttpPost]
    public async Task<ActionResult<InvestimentoResponse>> Criar([FromBody] CreateInvestimentoRequest request)
    {
        try {
            var investimento = await _investimentoService.CriarAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = investimento.Id }, investimento);
        } catch (ArgumentException ex) {
            return BadRequest(new { message = ex.Message });
        } catch (KeyNotFoundException ex) {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<InvestimentoResponse>>> Listar(
        [FromQuery] Guid? usuarioId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var resultado = await _investimentoService.ListarAsync(usuarioId, page, pageSize);
        return Ok(resultado);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<InvestimentoResponse>> GetById(Guid id)
    {
        var investimento = await _investimentoService.GetByIdAsync(id);

        if (investimento is null)
            return NotFound();

        return Ok(investimento);
    }

    [HttpPost("{id:guid}/sacar")]
    public async Task<ActionResult<InvestimentoResponse>> SacarInvestimento(
        Guid id,
        [FromBody] SacarInvestimentoRequest request)
    {
        try
        {
            var investimento = await _investimentoService.SacarAsync(id, request);
            return Ok(investimento);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
