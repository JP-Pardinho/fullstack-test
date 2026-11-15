import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { UsuarioService } from '../../services/usuario';

@Component({
  selector: 'app-usuario-criar',
  imports: [FormsModule, RouterLink],
  templateUrl: './usuario-criar.html',
  styleUrl: './usuario-criar.css',
})
export class UsuarioCriar {
  private readonly usuarioService = inject(UsuarioService);
  private readonly router = inject(Router);

  nome = signal('');
  enviando = signal(false);
  erro = signal<string | null>(null);

  salvar(): void {
    const nomeLimpo = this.nome().trim();

    if (nomeLimpo.length < 2) {
      this.erro.set('Informe um nome com pelo menos 2 caracteres.');
      return;
    }

    this.enviando.set(true);
    this.erro.set(null);

    this.usuarioService.criar({ nome: nomeLimpo }).subscribe({
      next: () => this.router.navigate(['/usuarios']),
      error: (err) => {
        this.erro.set(err?.error?.message ?? 'Não foi possível cadastrar o usuário.');
        this.enviando.set(false);
      },
    });
  }
}
