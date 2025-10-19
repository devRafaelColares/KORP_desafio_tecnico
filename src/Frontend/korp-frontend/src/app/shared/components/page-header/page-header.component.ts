import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-page-header',
    standalone: true,
    imports: [CommonModule],
    template: `
    <header class="page-header">
      <div class="header-content">
        <div class="header-info">
          <i *ngIf="icon" class="header-icon" [ngClass]="'icon-' + icon"></i>
          <div>
            <h1 class="header-title">{{ title }}</h1>
            <p *ngIf="subtitle" class="header-subtitle">{{ subtitle }}</p>
          </div>
        </div>
        <div class="header-actions">
          <ng-content></ng-content>
        </div>
      </div>
    </header>
  `,
    styles: [`
    .page-header {
      background: white;
      border-bottom: 1px solid #e5e7eb;
      box-shadow: 0 1px 3px 0 rgba(0, 0, 0, 0.1);
      margin-bottom: 2rem;
    }

    .header-content {
      max-width: 80rem;
      margin: 0 auto;
      padding: 1.5rem 2rem;
      display: flex;
      align-items: center;
      justify-content: space-between;
      gap: 1rem;
    }

    .header-info {
      display: flex;
      align-items: center;
      gap: 1rem;
    }

    .header-icon {
      font-size: 2rem;
      color: #2563eb;
    }

    .header-title {
      font-size: 1.875rem;
      font-weight: 700;
      color: #111827;
      margin: 0;
    }

    .header-subtitle {
      font-size: 0.875rem;
      color: #6b7280;
      margin: 0.25rem 0 0;
    }

    .header-actions {
      display: flex;
      gap: 0.75rem;
    }

    @media (max-width: 768px) {
      .header-content {
        flex-direction: column;
        align-items: flex-start;
      }

      .header-actions {
        width: 100%;
      }
    }
  `]
})
export class PageHeaderComponent {
    @Input() title = '';
    @Input() subtitle = '';
    @Input() icon = '';
}