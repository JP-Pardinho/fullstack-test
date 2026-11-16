namespace InvestmentApp.Api.Models
{
    public class Investimento
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public decimal ValorInicial { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataSaque { get; set; }
        public decimal? ValorTaxa { get; set; }

        public Investimento() {}

        public Investimento(Guid usuarioId, decimal valorInicial, DateTime dataCriacao)
        {
            Id = Guid.NewGuid();
            UsuarioId = usuarioId;
            ValorInicial = valorInicial;
            DataCriacao = dataCriacao;
        }

        public void Sacar(DateTime dataSaque, decimal valorTaxa)
        {
            DataSaque = dataSaque;
            ValorTaxa = valorTaxa;
        }
    }
}
