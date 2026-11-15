using InvestmentApp.Api.Services;

namespace InvestmentApp.Tests;

public class InvestimentoServiceTests
{
    [Fact]
    public void SaldoBruto_AntesDeUmMes()
    {
        var valorInicial = 1000m;
        var dataCriacao = new DateTime(2025, 1, 15);
        var dataReferencia = new DateTime(2025, 2, 14); 
        var saldo = InvestimentoService.CalcularSaldoBruto(valorInicial, dataCriacao, dataReferencia);
        Assert.Equal(1000m, saldo);
    }

    [Theory]                    // valores de imposto para R$200 de rendimento
    [InlineData(6, 45.00)]      //  < 1 ano       22,5% de 200 = 45,00 
    [InlineData(12, 37.00)]     //  1 a 2 anos    18,5% de 200 = 37,00
    [InlineData(25, 30.00)]     //  > 2 anos      15,0% de 200 = 30,00
    public void Impostar_PorInvervalo(int meses, decimal impostoEsperado)
    {
        var dataCriacao = new DateTime(2025, 1, 15);
        var dataSaque = dataCriacao.AddMonths(meses);
        var rendimento = 200m; // valor do rendimento para o teste

        var imposto = InvestimentoService.CalcularImpostos(rendimento, dataCriacao, dataSaque);

        Assert.Equal(impostoEsperado, imposto);
    }
}