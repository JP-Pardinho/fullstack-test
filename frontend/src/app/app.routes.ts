import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'investimentos', pathMatch: 'full' },

  {
    path: 'investimentos',
    loadComponent: () =>
      import('./pages/investimento-listar/investimento-listar').then((m) => m.InvestimentoListar),
  },
  {
    path: 'investimentos/novo',
    loadComponent: () =>
      import('./pages/investimento-criar/investimento-criar').then((m) => m.InvestimentoCriar),
  },
  {
    path: 'investimentos/:id',
    loadComponent: () =>
      import('./pages/investimento-visualizar/investimento-visualizar').then((m) => m.InvestimentoVisualizar),
  },
  {
  path: 'investimentos/:id/sacar',
  loadComponent: () =>
    import('./pages/investimento-sacar/investimento-sacar').then(
      (m) => m.InvestimentoSacar
    ),
  },
  {
    path: 'usuarios',
    loadComponent: () =>
      import('./pages/usuario-listar/usuario-listar').then((m) => m.UsuarioListar),
  },
  {
    path: 'usuarios/novo',
    loadComponent: () =>
      import('./pages/usuario-criar/usuario-criar').then((m) => m.UsuarioCriar),
  },
  {
    path: 'usuarios/:id',
    loadComponent: () =>
      import('./pages/usuario-visualizar/usuario-visualizar').then((m) => m.UsuarioVisualizar),
  },

  { path: '**', redirectTo: 'investimentos' },
];
