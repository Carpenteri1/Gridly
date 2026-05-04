import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BaseDialogComponent } from '../../../directives/base-dialog.directive';
import { DialogDirective } from '../../../directives/dialog.directive';

@Component({
  selector: 'app-delete-card-dialog',
  templateUrl: './delete-card-dialog.component.html',
  styleUrls: ['../../../css/shared.dialog.css'],
  standalone: true,
  imports: [FormsModule, DialogDirective],
})
export class DeleteCardDialogComponent extends BaseDialogComponent {
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
