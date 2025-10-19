import { Component, Input, forwardRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';

@Component({
    selector: 'app-input',
    standalone: true,
    imports: [CommonModule, FormsModule],
    providers: [{
        provide: NG_VALUE_ACCESSOR,
        useExisting: forwardRef(() => InputComponent),
        multi: true
    }],
    template: `
    <div class="input-wrapper">
      <label *ngIf="label" class="input-label">{{ label }}</label>
      <input
        [type]="type"
        [placeholder]="placeholder"
        [disabled]="disabled"
        [step]="step"
        [value]="value"
        (input)="onInputChange($event)"
        (blur)="onTouched()"
        [class.error]="!!error"
        class="input-field">
      <p *ngIf="error" class="error-message">
        <i class="icon-alert"></i>
        {{ error }}
      </p>
    </div>
  `,
    styles: [`
    .input-wrapper {
      margin-bottom: 1rem;
    }

    .input-label {
      display: block;
      font-size: 0.875rem;
      font-weight: 600;
      color: #374151;
      margin-bottom: 0.5rem;
    }

    .input-field {
      width: 100%;
      padding: 0.75rem 1rem;
      border: 1px solid #d1d5db;
      border-radius: 0.5rem;
      font-size: 1rem;
      transition: all 0.2s;

      &:focus {
        outline: none;
        border-color: #2563eb;
        box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
      }

      &.error {
        border-color: #dc2626;
        &:focus { border-color: #dc2626; box-shadow: 0 0 0 3px rgba(220, 38, 38, 0.1); }
      }

      &:disabled {
        background: #f3f4f6;
        cursor: not-allowed;
      }
    }

    .error-message {
      display: flex;
      align-items: center;
      gap: 0.25rem;
      margin-top: 0.5rem;
      font-size: 0.875rem;
      color: #dc2626;
    }
  `]
})
export class InputComponent implements ControlValueAccessor {
    @Input() label = '';
    @Input() type = 'text';
    @Input() placeholder = '';
    @Input() error = '';
    @Input() disabled = false;
    @Input() step = '1';

    value: any = '';
    onChange: any = () => { };
    onTouched: any = () => { };

    writeValue(value: any): void {
        this.value = value;
    }

    registerOnChange(fn: any): void {
        this.onChange = fn;
    }

    registerOnTouched(fn: any): void {
        this.onTouched = fn;
    }

    setDisabledState(isDisabled: boolean): void {
        this.disabled = isDisabled;
    }

    onInputChange(event: Event): void {
        const input = event.target as HTMLInputElement;
        this.value = this.type === 'number' ? +input.value : input.value;
        this.onChange(this.value);
    }
}