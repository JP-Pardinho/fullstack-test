using InvestmentApp.Api.Data;
using InvestmentApp.Api.DTOs;
using InvestmentApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Api.Services;

public class InvestimentoService
{
    private const double TaxaRendimentoMensal = 0.0052; 
    private readonly AppDbContext _context;
    public InvestimentoService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<InvestimentoResponse> CriarAsync(CreateInvestimentoRequest request)
    {
        if (request.DataCriacao.Date > DateTime.Today)
            throw new ArgumentException("A data de criação não pode ser uma data futura.");

        var usuario = await _context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == request.UsuarioId)
            ?? throw new KeyNotFoundException("Usuário não encontrado.");

        var investimento = new Investimento(request.UsuarioId, request.ValorInicial, request.DataCriacao.Date);

        _context.Investimentos.Add(investimento);
        await _context.SaveChangesAsync();

        return Response(investimento, usuario);
    }

    public async Task<PagedResponse<InvestimentoResponse>> ListarAsync(Guid? usuarioId, int page = 1, int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var query = _context.Investimentos
            .AsNoTracking()
            .Include(i => i.Usuario)
            .AsQueryable();

        if (usuarioId.HasValue)
            query = query.Where(i => i.UsuarioId == usuarioId.Value);

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var investimentos = await query
            .OrderByDescending(i => i.DataCriacao)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var items = investimentos
            .Select(i => Response(i, i.Usuario))
            .ToList();

        return new PagedResponse<InvestimentoResponse>(items, page, pageSize, totalItems, totalPages);
    }

    public async Task<InvestimentoResponse?> GetByIdAsync(Guid id)
    {
        var investimento = await _context.Investimentos
            .AsNoTracking()
            .Include(i => i.Usuario)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (investimento is null)
            return null;

        return Response(investimento, investimento.Usuario);
    }

    public static decimal CalcularSaldoBruto(decimal valorInicial, DateTime dataCriacao, DateTime dataReferencia)
    {
        var meses = (dataReferencia.Year - dataCriacao.Year) * 12 + (dataReferencia.Month - dataCriacao.Month);
        if (dataReferencia.Day < dataCriacao.Day)
            meses--;

        if (meses <= 0)
            return Math.Round(valorInicial, 2, MidpointRounding.AwayFromZero);

        var fator = (decimal)Math.Pow(1 + TaxaRendimentoMensal, meses);
        return Math.Round(valorInicial * fator, 2, MidpointRounding.AwayFromZero);
    }

    private static InvestimentoResponse Response(Investimento investimento, Usuario usuario)
    {
        var dataReferencia = investimento.DataSaque ?? DateTime.Today;
        var saldoBruto = CalcularSaldoBruto(investimento.ValorInicial, investimento.DataCriacao, dataReferencia);
        var rendimento = saldoBruto - investimento.ValorInicial;
        var saldoFinal = investimento.ValorTaxa.HasValue
            ? saldoBruto - investimento.ValorTaxa.Value
            : saldoBruto;

        return new InvestimentoResponse(
            investimento.Id,
            new UsuarioResponse(usuario.Id, usuario.Nome),
            investimento.ValorInicial,
            saldoFinal,
            rendimento,
            investimento.DataCriacao,
            investimento.DataSaque,
            investimento.ValorTaxa
        );
    }

    public static decimal CalcularImpostos(decimal rendimento, DateTime dataCriacao, DateTime dataSaque)
    {
        if (rendimento <= 0)
            return 0;

        decimal aliquota = 0;

        if (dataSaque.Date < dataCriacao.Date.AddYears(1)) {
            aliquota = 0.225m;
        } else if (dataSaque.Date <= dataCriacao.Date.AddYears(2)) {
            aliquota = 0.185m;
        } else {
            aliquota = 0.15m;
        }

        decimal imposto = Math.Round(rendimento * aliquota, 2, MidpointRounding.AwayFromZero);  
        
        return imposto;
    }

    public async Task<InvestimentoResponse> SacarAsync(Guid id, SacarInvestimentoRequest request)
    {
        var investimento = await _context.Investimentos
            .Include(i => i.Usuario)
            .FirstOrDefaultAsync(i => i.Id == id)
            ?? throw new KeyNotFoundException("Investimento não encontrado.");

        if (investimento.DataSaque.HasValue)
            throw new InvalidOperationException("O investimento já foi sacado.");

        if (request.DataSaque.Date < investimento.DataCriacao.Date || request.DataSaque.Date > DateTime.Today)
            throw new ArgumentException("A data de saque não pode ser anterior à criação nem futura.");

        var saldoBruto = CalcularSaldoBruto(investimento.ValorInicial, investimento.DataCriacao, request.DataSaque);
        var rendimento = saldoBruto - investimento.ValorInicial;
        var imposto = CalcularImpostos(rendimento, investimento.DataCriacao, request.DataSaque);

        investimento.DataSaque = request.DataSaque.Date;
        investimento.ValorTaxa = imposto;

        await _context.SaveChangesAsync();

        return Response(investimento, investimento.Usuario);
    }

}
