import { Routes } from '@angular/router';

export const routes: Routes = [
    {
        path: '',
        redirectTo: '/produtos',
        pathMatch: 'full'
    },
    {
        path: 'produtos',
        loadChildren: () =>
            import('./features/produtos/produtos.routes')
                .then(m => m.produtosRoutes)
    },
    // {
    //     path: 'notas-fiscais',
    //     loadChildren: () =>
    //         import('./features/notas-fiscais/notas-fiscais.routes')
    //             .then(m => m.notasFiscaisRoutes)
    // },
    {
        path: '**',
        redirectTo: '/produtos'
    }
];