using System.ComponentModel.DataAnnotations;

namespace InvestmentApp.Api.DTOs;

public record CreateUsuarioRequest(
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 150 caracteres.")]
    string Nome
);

public record UsuarioResponse(Guid Id, string Nome);
