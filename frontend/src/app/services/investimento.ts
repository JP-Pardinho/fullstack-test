import {
  CreateInvestimentoRequest,
  Investimento,
  SacarInvestimentoRequest,
  PagedResponse
} from './../models/investimento.model';
import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environments';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })

export class InvestimentoService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/investimentos`;

  listar(usuarioId?: string, page=1, pageSize=10): Observable<PagedResponse<Investimento>> {
    let params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize);

    if (usuarioId) {
      params = params.set('usuarioId', usuarioId);
    }

    return this.http.get<PagedResponse<Investimento>>(this.apiUrl, { params });
  }

  criar(payload: CreateInvestimentoRequest): Observable<Investimento> {
    return this.http.post<Investimento>(this.apiUrl, payload);
  }

  getById(id: string): Observable<Investimento> {
    return this.http.get<Investimento>(`${this.apiUrl}/${id}`);
  }

  sacar(payload: SacarInvestimentoRequest, id: string): Observable<Investimento> {
    return this.http.post<Investimento>(`${this.apiUrl}/${id}/sacar`, payload);
  }
}
