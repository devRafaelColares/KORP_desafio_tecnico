import { Routes } from '@angular/router';

export const routes: Routes = [
    {
        path: '',
        loadComponent: () =>
            import('./layout/main-layout/main-layout.component')
                .then(m => m.MainLayoutComponent),
        children: [
            {
                path: '',
                loadComponent: () =>
                    import('./features/home/home.component')
                        .then(m => m.HomeComponent),
                title: 'Home - KORP'
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
            }
        ]
    },
    {
        path: '**',
        redirectTo: ''
    }
];