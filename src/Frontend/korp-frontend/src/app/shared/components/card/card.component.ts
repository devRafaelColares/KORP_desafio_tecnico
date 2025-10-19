import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-card',
    standalone: true,
    imports: [CommonModule],
    template: `
    <div class="card">
      <ng-content></ng-content>
    </div>
  `,
    styles: [`
    .card {
      background: white;
      border-radius: 0.75rem;
      box-shadow: 0 1px 3px 0 rgba(0, 0, 0, 0.1);
      padding: 1.5rem;
      transition: box-shadow 0.2s;

      &:hover {
        box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
      }
    }
  `]
})
export class CardComponent { }