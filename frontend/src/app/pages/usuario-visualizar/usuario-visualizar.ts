import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Investimento } from '../../models/investimento.model';
import { Usuario } from '../../models/usuario.model';
import { InvestimentoService } from '../../services/investimento';
import { UsuarioService } from '../../services/usuario';

@Component({
  selector: 'app-usuario-visualizar',
  imports: [CurrencyPipe, DatePipe, RouterLink],
  templateUrl: './usuario-visualizar.html',
  styleUrl: './usuario-visualizar.css',
})
export class UsuarioVisualizar implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly usuarioService = inject(UsuarioService);
  private readonly investimentoService = inject(InvestimentoService);

  usuario = signal<Usuario | null>(null);
  investimentos = signal<Investimento[]>([]);
  saldoTotal = computed(() =>
    this.investimentos()
      .filter((i) => !i.dataSaque)
      .reduce((total, i) => total + i.saldoAtual, 0)
  );
  page = signal(1);
  totalPages = signal(1);
  totalItems = signal(0);
  carregando = signal(true);
  erro = signal<string | null>(null);

  private usuarioId = '';

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    
    if (!id) {
      this.erro.set('ID do usuário inválido.');
      this.carregando.set(false);
      return;
    }

    this.usuarioId = id;
    this.carregarUsuario();
    this.carregarInvestimentos();
  }

  private carregarUsuario(): void {
    this.usuarioService.getById(this.usuarioId).subscribe({
      next: (dados) => {
        this.usuario.set(dados);
        this.carregando.set(false);
      },
      error: () => {
        this.erro.set('Usuário não encontrado.');
        this.carregando.set(false);
      },
    });
  }

  carregarInvestimentos(): void {
    this.investimentoService.listar(this.usuarioId, this.page(), 5).subscribe({
      next: (res) => {
        this.investimentos.set(res.items);
        this.totalPages.set(res.totalPages || 1);
        this.totalItems.set(res.totalItems);
      },
    });
  }

  mudarPagina(novaPagina: number): void {
    if (novaPagina < 1 || novaPagina > this.totalPages()) return;
    this.page.set(novaPagina);
    this.carregarInvestimentos();
  }
}
