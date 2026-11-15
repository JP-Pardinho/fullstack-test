import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Investimento } from '../../models/investimento.model';
import { InvestimentoService } from '../../services/investimento';

@Component({
  selector: 'app-investimento-visualizar',
  imports: [CurrencyPipe, DatePipe, RouterLink],
  templateUrl: './investimento-visualizar.html',
  styleUrl: './investimento-visualizar.css',
})
export class InvestimentoVisualizar implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly investimentoService = inject(InvestimentoService);

  investimento = signal<Investimento | null>(null);

  carregando = signal(true);
  erro = signal<string | null>(null);

  private investimentoId = '';

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (!id) {
      this.erro.set('ID do investimento inválido.');
      this.carregando.set(false);
      return;
    }

    this.investimentoId = id;
    this.carregarInvestimento();
  }

  carregarInvestimento(): void {
    this.investimentoService.getById(this.investimentoId).subscribe({
      next: (dados) => {
        this.investimento.set(dados);
        this.carregando.set(false);
      },
      error: () => {
        this.erro.set('Investimento não encontrado.');
        this.carregando.set(false);
      }
    })
  }
}
