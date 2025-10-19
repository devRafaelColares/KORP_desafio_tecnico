import { Routes } from '@angular/router';

export const notasFiscaisRoutes: Routes = [
    {
        path: '',
        redirectTo: 'pdv',
        pathMatch: 'full'
    },
    {
        path: 'pdv',
        loadComponent: () =>
            import('./components/pdv/pdv.component')
                .then(m => m.PdvComponent),
        title: 'PDV - Ponto de Venda'
    },
    {
        path: 'lista',
        loadComponent: () =>
            import('./components/notas-list/notas-list.component')
                .then(m => m.NotasListComponent),
        title: 'Notas Fiscais'
    }
];
