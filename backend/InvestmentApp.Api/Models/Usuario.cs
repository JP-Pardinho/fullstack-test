namespace InvestmentApp.Api.Models
{
    public class Usuario
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public ICollection<Investimento> Investimentos { get; private set; } = new List<Investimento>();

        public Usuario() {}

        public Usuario(string nome)
        {
            Id = Guid.NewGuid();
            Nome = nome;
        }

    }
}