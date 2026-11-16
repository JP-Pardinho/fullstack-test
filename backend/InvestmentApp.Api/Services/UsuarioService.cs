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
        var nomeNormalizado = request.Nome.Trim();

        var nomeJaExiste = await _context.Usuarios
            .AnyAsync(u => u.Nome.ToLower() == nomeNormalizado.ToLower());

        if (nomeJaExiste)
            throw new InvalidOperationException("Já existe um usuário cadastrado com este nome.");

        var usuario = new Usuario(nomeNormalizado);

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

    public async Task ExcluirAsync(Guid id)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id)
            ?? throw new KeyNotFoundException("Usuário não encontrado.");

        var possuiInvestimentos = await _context.Investimentos.AnyAsync(i => i.UsuarioId == id);
        if (possuiInvestimentos)
            throw new InvalidOperationException("Não é possível excluir um usuário que possui investimentos cadastrados (ativos ou sacados).");

        _context.Usuarios.Remove(usuario);
        await _context.SaveChangesAsync();
    }
}
