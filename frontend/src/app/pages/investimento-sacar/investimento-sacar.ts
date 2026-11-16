import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Investimento } from '../../models/investimento.model';
import { InvestimentoService } from '../../services/investimento';

@Component({
  selector: 'app-investimento-sacar',
  imports: [CurrencyPipe, DatePipe, FormsModule, RouterLink],
  templateUrl: './investimento-sacar.html',
  styleUrl: './investimento-sacar.css',
})
export class InvestimentoSacar implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly investimentoService = inject(InvestimentoService);

  investimento = signal<Investimento | null>(null);
  dataSaque = signal(new Date().toISOString().split('T')[0]);
  carregando = signal(true);
  enviando = signal(false);
  erro = signal<string | null>(null);
  saqueRealizado = signal(false);

  readonly hoje = new Date().toISOString().split('T')[0];

  dataMinima = computed(() => this.investimento()?.dataCriacao.split('T')[0] ?? '');

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

  private carregarInvestimento(): void {
    this.investimentoService.getById(this.investimentoId).subscribe({
      next: (dados) => {
        this.investimento.set(dados);
        this.carregando.set(false);
      },
      error: () => {
        this.erro.set('Investimento não encontrado.');
        this.carregando.set(false);
      },
    });
  }

  sacar(): void {
    const data = this.dataSaque();
    const min = this.dataMinima();

    if (!data) {
      this.erro.set('Informe a data do saque.');
      return;
    }

    if (min && data < min) {
      this.erro.set('A data do saque não pode ser anterior à data de criação.');
      return;
    }

    if (data > this.hoje) {
      this.erro.set('A data do saque não pode ser futura.');
      return;
    }

    this.enviando.set(true);
    this.erro.set(null);

    this.investimentoService.sacar({ dataSaque: data }, this.investimentoId).subscribe({
      next: (atualizado) => {
        this.investimento.set(atualizado);
        this.saqueRealizado.set(true);
        this.enviando.set(false);
      },
      error: (err) => {
        this.erro.set(err?.error?.message ?? 'Não foi possível realizar o saque.');
        this.enviando.set(false);
      },
    });
  }
}
