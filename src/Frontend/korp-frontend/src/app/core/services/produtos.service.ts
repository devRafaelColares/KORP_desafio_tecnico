import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
    Produto,
    ProdutoCreateRequest,
    ProdutoUpdateRequest
} from '../models/produto.model';
import { Response, PagedResponse } from '../models/response.model';

@Injectable({
    providedIn: 'root'
})
export class ProdutosService {
    private readonly apiUrl = `${environment.estoqueServiceUrl}/produtos`;

    constructor(private http: HttpClient) { }

    getAll(pageNumber = 1, pageSize = 10): Observable<PagedResponse<Produto[]>> {
        const params = new HttpParams()
            .set('pageNumber', pageNumber.toString())
            .set('pageSize', pageSize.toString());

        return this.http.get<PagedResponse<Produto[]>>(this.apiUrl, { params });
    }

    getById(id: number): Observable<Response<Produto>> {
        return this.http.get<Response<Produto>>(`${this.apiUrl}/${id}`);
    }

    getBySku(sku: string): Observable<Response<Produto>> {
        return this.http.get<Response<Produto>>(`${this.apiUrl}/sku/${sku}`);
    }

    create(produto: ProdutoCreateRequest): Observable<Response<Produto>> {
        return this.http.post<Response<Produto>>(this.apiUrl, produto);
    }

    update(id: number, produto: ProdutoUpdateRequest): Observable<Response<Produto>> {
        return this.http.put<Response<Produto>>(`${this.apiUrl}/${id}`, produto);
    }

    delete(id: number): Observable<Response<void>> {
        return this.http.delete<Response<void>>(`${this.apiUrl}/${id}`);
    }
}