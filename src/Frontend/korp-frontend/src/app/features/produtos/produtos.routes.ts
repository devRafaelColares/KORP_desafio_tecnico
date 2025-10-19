import { Routes } from '@angular/router';

export const produtosRoutes: Routes = [
    {
        path: '',
        loadComponent: () =>
            import('./components/produtos-list/produtos-list.component')
                .then(m => m.ProdutosListComponent)
    }
];