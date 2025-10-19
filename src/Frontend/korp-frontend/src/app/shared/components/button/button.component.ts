import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'button[korp-button]',
  standalone: true,
  imports: [CommonModule],
  template: `<ng-content></ng-content>`,
  styles: [`
    :host {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      gap: 0.5rem;
      padding: 0.75rem 1.5rem;
      font-weight: 600;
      border-radius: 0.5rem;
      transition: all 0.2s;
      cursor: pointer;
      border: none;
      outline: none;

      &:disabled {
        opacity: 0.5;
        cursor: not-allowed;
      }

      &.primary {
        background: #2563eb;
        color: white;
        &:hover:not(:disabled) { background: #1d4ed8; }
      }

      &.secondary {
        background: #e5e7eb;
        color: #374151;
        &:hover:not(:disabled) { background: #d1d5db; }
      }

      &.danger {
        background: #dc2626;
        color: white;
        &:hover:not(:disabled) { background: #b91c1c; }
      }

      &.success {
        background: #16a34a;
        color: white;
        &:hover:not(:disabled) { background: #15803d; }
      }
    }
  `]
})
export class ButtonComponent {
  @Input() variant: 'primary' | 'secondary' | 'danger' | 'success' = 'primary';
  @Input() disabled = false;
}