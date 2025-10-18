import { Routes } from '@angular/router';

export const routes: Routes = [
    {
        path: 'produtos',
        loadChildren: () => import('./features/produtos/produtos.routes').then(m => m.default)
    },
    {
        path: 'notas',
        loadChildren: () => import('./features/notas/notas.routes').then(m => m.default)
    },
    { path: '', redirectTo: '/produtos', pathMatch: 'full' }
];