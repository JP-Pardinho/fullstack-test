using InvestmentApp.Api.DTOs;
using InvestmentApp.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvestmentApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly UsuarioService _usuarioService;

    public UsuariosController(UsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpPost]
    public async Task<ActionResult<UsuarioResponse>> Criar(CreateUsuarioRequest request)
    {
        try
        {
            var usuario = await _usuarioService.CriarAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<UsuarioResponse>>> Listar()
    {
        var usuarios = await _usuarioService.ListarAsync();
        return Ok(usuarios);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UsuarioResponse>> GetById(Guid id)
    {
        var usuario = await _usuarioService.GetByIdAsync(id);

        if (usuario is null)
            return NotFound();

        return Ok(usuario);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id)
    {
        try
        {
            await _usuarioService.ExcluirAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
