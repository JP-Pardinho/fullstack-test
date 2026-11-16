import { Usuario } from './usuario.model';
export interface Investimento {
  id: string;
  usuario: Usuario;
  valorInicial: number;
  saldoAtual: number;
  rendimento: number;
  dataCriacao: string;
  dataSaque: string | null;
  valorTaxa: number | null;
}

export interface CreateInvestimentoRequest {
  usuarioId: string;
  valorInicial: number;
  dataCriacao: string;
}

export interface SacarInvestimentoRequest {
  dataSaque: string
}

export interface PagedResponse<T> {
  items: T[],
  page: number,
  pageSize: number,
  totalItems: number,
  totalPages: number
}


