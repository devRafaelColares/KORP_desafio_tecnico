import { Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class HttpErrorHandlerService {
    handleError(error: HttpErrorResponse): Observable<never> {
        let errorMessage = 'Ocorreu um erro inesperado.';

        if (error.error instanceof ErrorEvent) {
            // Erro do cliente
            errorMessage = `Erro: ${error.error.message}`;
        } else {
            // Erro do servidor
            if (error.status === 0) {
                errorMessage = 'Não foi possível conectar ao servidor. Verifique sua conexão.';
            } else if (error.status === 400) {
                errorMessage = error.error?.message || 'Requisição inválida.';
            } else if (error.status === 404) {
                errorMessage = 'Recurso não encontrado.';
            } else if (error.status === 409) {
                errorMessage = error.error?.message || 'Conflito de dados.';
            } else if (error.status === 500) {
                errorMessage = 'Erro interno do servidor. Tente novamente mais tarde.';
            } else {
                errorMessage = error.error?.message || `Erro ${error.status}: ${error.statusText}`;
            }
        }

        console.error('Erro HTTP:', error);
        return throwError(() => new Error(errorMessage));
    }
}