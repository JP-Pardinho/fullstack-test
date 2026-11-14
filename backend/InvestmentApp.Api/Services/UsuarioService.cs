using InvestmentApp.Api.Data;
using InvestmentApp.Api.DTOs;
using InvestmentApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Api.Services;

public class UsuarioService
{
    private readonly AppDbContext _context;

    public UsuarioService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UsuarioResponse> CriarAsync(CreateUsuarioRequest request)
    {
        var usuario = new Usuario(request.Nome.Trim());

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return new UsuarioResponse(usuario.Id, usuario.Nome);
    }

    public async Task<List<UsuarioResponse>> ListarAsync()
    {
        return await _context.Usuarios
            .AsNoTracking()
            .OrderBy(u => u.Nome)
            .Select(u => new UsuarioResponse(u.Id, u.Nome))
            .ToListAsync();
    }

    public async Task<UsuarioResponse?> GetByIdAsync(Guid id)
    {
        return await _context.Usuarios
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UsuarioResponse(u.Id, u.Nome))
            .FirstOrDefaultAsync();
    }
}
