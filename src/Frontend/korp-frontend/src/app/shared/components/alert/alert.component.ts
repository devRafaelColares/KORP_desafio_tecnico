import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-alert',
    standalone: true,
    imports: [CommonModule],
    template: `
    <div class="alert" [ngClass]="'alert-' + type">
      <div class="alert-content">
        <i class="alert-icon" [ngClass]="getIcon()"></i>
        <p class="alert-message">{{ message }}</p>
      </div>
      <button *ngIf="closeable" class="alert-close" (click)="close.emit()">
        <i class="icon-x"></i>
      </button>
    </div>
  `,
    styles: [`
    .alert {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 1rem;
      border-radius: 0.5rem;
      border-left: 4px solid;
      margin-bottom: 1rem;
    }

    .alert-content {
      display: flex;
      align-items: center;
      gap: 0.75rem;
    }

    .alert-message {
      font-weight: 500;
      margin: 0;
    }

    .alert-close {
      background: none;
      border: none;
      cursor: pointer;
      padding: 0.25rem;
      transition: opacity 0.2s;
      &:hover { opacity: 0.7; }
    }

    .alert-success {
      background: #dcfce7;
      border-color: #16a34a;
      color: #166534;
    }

    .alert-error {
      background: #fee2e2;
      border-color: #dc2626;
      color: #991b1b;
    }

    .alert-info {
      background: #dbeafe;
      border-color: #2563eb;
      color: #1e40af;
    }
  `]
})
export class AlertComponent {
    @Input() type: 'success' | 'error' | 'info' = 'info';
    @Input() message = '';
    @Input() closeable = true;
    @Output() close = new EventEmitter<void>();

    getIcon(): string {
        const icons = {
            success: 'icon-check-circle',
            error: 'icon-alert-circle',
            info: 'icon-info'
        };
        return icons[this.type];
    }
}