using System.ComponentModel.DataAnnotations;

namespace InvestmentApp.Api.DTOs;

public record CreateInvestimentoRequest(
    [Required(ErrorMessage = "O Id do usuário é obrigatório.")]
    Guid UsuarioId,

    [Required(ErrorMessage = "O valor inicial é obrigatório.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor inicial deve ser maior que zero.")]
    decimal ValorInicial,

    [Required(ErrorMessage = "A data de criação é obrigatória.")]
    DateTime DataCriacao
);

public record SacarInvestimentoRequest(
    [Required(ErrorMessage = "A data de saque é obrigatória.")]
    DateTime DataSaque
);

public record InvestimentoResponse(
    Guid Id,
    UsuarioResponse Usuario,
    decimal ValorInicial,
    decimal SaldoAtual,
    decimal Rendimento,
    DateTime DataCriacao,
    DateTime? DataSaque,
    decimal? ValorTaxa
);

public record PagedResponse<T>(
    List<T> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages
);
