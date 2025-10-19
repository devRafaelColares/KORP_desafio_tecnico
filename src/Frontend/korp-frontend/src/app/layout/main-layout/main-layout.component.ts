import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';

@Component({
    selector: 'app-main-layout',
    standalone: true,
    imports: [CommonModule, RouterModule],
    templateUrl: './main-layout.component.html',
    styleUrls: ['./main-layout.component.scss']
})
export class MainLayoutComponent {
    isMenuOpen = false;

    constructor(private router: Router) { }

    toggleMenu(): void {
        this.isMenuOpen = !this.isMenuOpen;
    }

    navigateTo(route: string): void {
        this.router.navigate([route]);
        this.isMenuOpen = false;
    }

    isActive(route: string): boolean {
        return this.router.url.startsWith(route);
    }
}