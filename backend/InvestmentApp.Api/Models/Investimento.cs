namespace InvestmentApp.Api.Models
{
    public class Investimento
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public decimal ValorInicial { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataSaque { get; set; }
        public decimal? ValorTaxa { get; set; }

        public Investimento() {}

        public Investimento(Guid userId, decimal valorInicial, DateTime dataCriacao)
        {
            Id = Guid.NewGuid();
            UserId = userId;
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