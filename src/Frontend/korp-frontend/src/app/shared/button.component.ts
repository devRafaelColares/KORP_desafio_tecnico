import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
    selector: 'korp-button',
    standalone: true,
    template: `
    <button
      [class]="variant"
      [disabled]="disabled"
      (click)="onClick.emit($event)">
      <ng-content></ng-content>
    </button>
  `,
    styles: [`
    button { @apply px-4 py-2 rounded font-semibold transition; }
    .primary { @apply bg-blue-600 text-white hover:bg-blue-700; }
    .secondary { @apply bg-gray-200 text-gray-800 hover:bg-gray-300; }
  `]
})
export class ButtonComponent {
    @Input() variant: 'primary' | 'secondary' = 'primary';
    @Input() disabled = false;
    @Output() onClick = new EventEmitter<Event>();
}