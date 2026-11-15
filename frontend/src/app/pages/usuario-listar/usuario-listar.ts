import { RouterLink } from '@angular/router';
import { Usuario } from '../../models/usuario.model';
import { UsuarioService } from './../../services/usuario';
import { Component, inject, OnInit, signal } from '@angular/core';
import { LucideAngularModule } from 'lucide-angular';

@Component({
  selector: 'app-usuario-listar',
  imports: [RouterLink, LucideAngularModule],
  templateUrl: './usuario-listar.html',
  styleUrl: './usuario-listar.css',
})
export class UsuarioListar implements OnInit {
  private readonly usuarioService = inject(UsuarioService);

  usuarios = signal<Usuario[]>([]);
  carregando = signal(true);
  erro = signal<string | null>(null);

  ngOnInit(): void {
    this.usuarioService.listar().subscribe({
      next: (dados) => {
        this.usuarios.set(dados);
        this.carregando.set(false);
      },
      error: () => {
        this.erro.set('Erro ao carregar os usuários.');
        this.carregando.set(false);
      }
    });
  }

  excluir(usuario: Usuario): void {
    if (!confirm(`Deseja realmente excluir o usuário "${usuario.nome}"?`)) return;

    this.erro.set(null);
    this.usuarioService.excluir(usuario.id).subscribe({
      next: () => {
        this.usuarios.update((lista) => lista.filter((u) => u.id !== usuario.id));
      },
      error: (err) => {
        this.erro.set(err?.error?.message ?? 'Não foi possível excluir o usuário.');
      },
    });
  }
}
