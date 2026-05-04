import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BaseModalComponent } from '../../../directives/base-modal.component';

@Component({
  selector: 'app-prompt-dialog',
  templateUrl: './prompt.dialog.component.html',
  styleUrls: ['../../../css/shared.modal.css'],
  standalone: true,
  imports: [FormsModule],
})
export class PromptModalComponent extends BaseModalComponent {
  @Input() open = false;
  @Input() cardId = 0;
  @Input() id = 0;
  @Output() openChange = new EventEmitter<number>();
  @Output() remove = new EventEmitter<{id: number}>();

  onSubmit(): void {
    this.close();
    this.remove.emit({ id: this.id });
  }
}
