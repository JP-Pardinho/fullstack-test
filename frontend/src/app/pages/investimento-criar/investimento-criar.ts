import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Usuario } from '../../models/usuario.model';
import { InvestimentoService } from '../../services/investimento';
import { UsuarioService } from '../../services/usuario';

@Component({
  selector: 'app-investimento-criar',
  imports: [FormsModule, RouterLink],
  templateUrl: './investimento-criar.html',
  styleUrl: './investimento-criar.css',
})
export class InvestimentoCriar implements OnInit {
  private readonly investimentoService = inject(InvestimentoService);
  private readonly usuarioService = inject(UsuarioService);
  private readonly router = inject(Router);

  usuarios = signal<Usuario[]>([]);
  usuarioId = signal('');
  valorInicial = signal<number | null>(null);
  dataCriacao = signal(new Date().toISOString().split('T')[0]);

  enviando = signal(false);
  erro = signal<string | null>(null);

  readonly hoje = new Date().toISOString().split('T')[0];

  ngOnInit(): void {
    this.usuarioService.listar().subscribe({
      next: (dados) => this.usuarios.set(dados),
      error: () => this.erro.set('Não foi possível carregar a lista de usuários.'),
    });
  }

  salvar(): void {
    const usuarioId = this.usuarioId();
    const valor = Number(this.valorInicial());
    const data = this.dataCriacao();

    if (!usuarioId) {
      this.erro.set('Selecione um titular para o investimento.');
      return;
    }

    if (!valor || valor <= 0) {
      this.erro.set('Informe um valor inicial maior que zero.');
      return;
    }

    if (!data) {
      this.erro.set('Informe a data de criação.');
      return;
    }

    if (data > this.hoje) {
      this.erro.set('A data de criação não pode ser futura.');
      return;
    }

    this.enviando.set(true);
    this.erro.set(null);

    this.investimentoService
      .criar({
        usuarioId,
        valorInicial: valor,
        dataCriacao: data,
      })
      .subscribe({
        next: () => this.router.navigate(['/investimentos']),
        error: (err) => {
          this.erro.set(err?.error?.message ?? 'Não foi possível criar o investimento.');
          this.enviando.set(false);
        },
      });
  }
}
