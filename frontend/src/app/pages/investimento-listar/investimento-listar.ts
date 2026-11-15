import { LucideAngularModule } from 'lucide-angular';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Investimento } from '../../models/investimento.model';
import { Usuario } from '../../models/usuario.model';
import { InvestimentoService } from '../../services/investimento';
import { UsuarioService } from '../../services/usuario';

@Component({
  selector: 'app-investimento-listar',
  imports: [CurrencyPipe, DatePipe, RouterLink, LucideAngularModule],
  templateUrl: './investimento-listar.html',
  styleUrl: './investimento-listar.css',
})
export class InvestimentoListar implements OnInit {
  private readonly investimentoService = inject(InvestimentoService);
  private readonly usuarioService = inject(UsuarioService);

  investimentos = signal<Investimento[]>([]);
  usuarios = signal<Usuario[]>([]);
  usuarioSelecionado = signal<string>('');
  saldoTotal = signal(0);
  page = signal(1);
  totalPages = signal(1);
  totalItems = signal(0);
  carregando = signal(true);
  erro = signal<string | null>(null);

  ngOnInit(): void {
    this.carregarUsuarios();
    this.carregarInvestimentos();
    this.carregarSaldoTotal();
  }

  carregarUsuarios(): void {
    this.usuarioService.listar().subscribe({
      next: (dados) => this.usuarios.set(dados),
      error: () => this.erro.set('Não foi possível carregar os usuários.'),
    });
  }

  carregarInvestimentos(): void {
    this.carregando.set(true);
    this.erro.set(null);

    const usuarioId = this.usuarioSelecionado() || undefined;
    this.investimentoService.listar(usuarioId, this.page(), 5).subscribe({
      next: (res) => {
        this.investimentos.set(res.items);
        this.totalPages.set(res.totalPages || 1);
        this.totalItems.set(res.totalItems);
        this.carregando.set(false);
      },
      error: () => {
        this.erro.set('Erro ao carregar investimentos. Verifique se a API está rodando.');
        this.carregando.set(false);
      },
    });
  }

  carregarSaldoTotal(): void {
    const usuarioId = this.usuarioSelecionado() || undefined;
    this.investimentoService.listar(usuarioId, 1, 100).subscribe({
      next: (res) => {
        const total = res.items
          .filter((i) => !i.dataSaque)
          .reduce((acc, i) => acc + i.saldoAtual, 0);
        this.saldoTotal.set(total);
      },
    });
  }

  filtrarPorUsuario(usuarioId: string): void {
    this.usuarioSelecionado.set(usuarioId);
    this.page.set(1);
    this.carregarInvestimentos();
    this.carregarSaldoTotal();
  }

  mudarPagina(novaPagina: number): void {
    if (novaPagina < 1 || novaPagina > this.totalPages()) return;
    this.page.set(novaPagina);
    this.carregarInvestimentos();
  }
}
