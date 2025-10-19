import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
    NotaFiscal,
    NotaFiscalCreateRequest,
    ImprimirNotaFiscalResponse
} from '../models/nota-fiscal.model';
import { Response, PagedResponse } from '../models/response.model';

@Injectable({
    providedIn: 'root'
})
export class NotasFiscaisService {
    private readonly apiUrl = `${environment.faturamentoServiceUrl}/notas-fiscais`;

    constructor(private http: HttpClient) { }

    getAll(pageNumber = 1, pageSize = 10): Observable<PagedResponse<NotaFiscal[]>> {
        const params = new HttpParams()
            .set('pageNumber', pageNumber.toString())
            .set('pageSize', pageSize.toString());

        return this.http.get<PagedResponse<NotaFiscal[]>>(this.apiUrl, { params });
    }

    getById(id: number): Observable<Response<NotaFiscal>> {
        return this.http.get<Response<NotaFiscal>>(`${this.apiUrl}/${id}`);
    }

    create(nota: NotaFiscalCreateRequest): Observable<Response<NotaFiscal>> {
        return this.http.post<Response<NotaFiscal>>(this.apiUrl, nota);
    }

    imprimir(notaFiscalId: number): Observable<Response<ImprimirNotaFiscalResponse>> {
        return this.http.post<Response<ImprimirNotaFiscalResponse>>(
            `${this.apiUrl}/${notaFiscalId}/imprimir`,
            {}
        );
    }

    update(id: number, data: { itens: { produtoId: number; quantidade: number }[] }): Observable<Response<NotaFiscal>> {
        return this.http.put<Response<NotaFiscal>>(`${this.apiUrl}/${id}`, data);
    }

    delete(id: number): Observable<Response<void>> {
        return this.http.delete<Response<void>>(`${this.apiUrl}/${id}`);
    }
}