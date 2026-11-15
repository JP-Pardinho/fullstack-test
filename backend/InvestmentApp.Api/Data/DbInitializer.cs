using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvestmentApp.Api.Models;
using InvestmentApp.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Api.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.Usuarios.AnyAsync())
                return;

            var usuarios = new List<Usuario>
            {
                new Usuario {Id = Guid.NewGuid(), Nome = "Maria Santos"},
                new Usuario {Id = Guid.NewGuid(), Nome = "Joaquim da Silva"},
                new Usuario {Id = Guid.NewGuid(), Nome = "Ana Rodrigues"}
            };
            await context.Usuarios.AddRangeAsync(usuarios);
            await context.SaveChangesAsync();

            var investimentos = new List<Investimento>
            {
                new Investimento(usuarios[0].Id, 10500m, DateTime.Today.AddMonths(-6)),
                new Investimento(usuarios[1].Id, 20500m, DateTime.Today.AddMonths(-18)),
                new Investimento(usuarios[2].Id, 30500m, DateTime.Today.AddMonths(-30)),
                
                new Investimento(usuarios[0].Id, 1500m, DateTime.Today.AddMonths(-6)),
                new Investimento(usuarios[1].Id, 2500m, DateTime.Today.AddMonths(-18)),
                new Investimento(usuarios[2].Id, 3500m, DateTime.Today.AddMonths(-30)),

                new Investimento(usuarios[0].Id, 100m, DateTime.Today.AddMonths(-6)),
                new Investimento(usuarios[1].Id, 200m, DateTime.Today.AddMonths(-18)),
                new Investimento(usuarios[2].Id, 300m, DateTime.Today.AddMonths(-30)),

            };

            foreach (var inv in investimentos.Take(3))
            {
                var saldoBruto = InvestimentoService.CalcularSaldoBruto(inv.ValorInicial, inv.DataCriacao, DateTime.Today);
                var rendimento = saldoBruto - inv.ValorInicial;
                var taxa = InvestimentoService.CalcularImpostos(rendimento, inv.DataCriacao, DateTime.Today);
                inv.Sacar(DateTime.Today, taxa);
            }

            await context.Investimentos.AddRangeAsync(investimentos);
            await context.SaveChangesAsync();


        }
    }
}