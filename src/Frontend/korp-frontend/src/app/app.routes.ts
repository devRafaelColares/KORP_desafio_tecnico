import { Routes } from '@angular/router';

export const routes: Routes = [
    {
        path: '',
        redirectTo: '/pdv',
        pathMatch: 'full'
    },
    {
        path: 'pdv',
        loadChildren: () =>
            import('./features/notas-fiscais/notas-fiscais.routes')
                .then(m => m.notasFiscaisRoutes),
        title: 'PDV'
    },
    {
        path: 'notas-fiscais',
        loadChildren: () =>
            import('./features/notas-fiscais/notas-fiscais.routes')
                .then(m => m.notasFiscaisRoutes),
        title: 'Notas Fiscais'
    },
    {
        path: 'produtos',
        loadChildren: () =>
            import('./features/produtos/produtos.routes')
                .then(m => m.produtosRoutes),
        title: 'Produtos'
    },
    {
        path: '**',
        redirectTo: '/pdv'
    }
];