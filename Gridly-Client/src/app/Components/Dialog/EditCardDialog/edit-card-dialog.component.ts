import { Component, EventEmitter, inject, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ModalDirective } from '../../../Directives/modal.directive';
import { BaseModalComponent } from '../../../Directives/base-modal.component';
import { CardModel } from '../../../Models/card.Model';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { EditCardDialogFacade } from './edit-card-dialog.facade';

@Component({
  selector: 'app-edit-card-dialog',
  imports: [CommonModule, FormsModule, ModalDirective, MatIconModule, MatSelectModule, MatInputModule],  
  templateUrl: './edit-card-dialog.component.html',
  styleUrls: ['../../../css/shared.modal.css', './edit-card-dialog.component.css'],
  providers: [EditCardDialogFacade],
  standalone: true
})
export class EditCardDialogComponent extends BaseModalComponent implements OnChanges {
  @Input() open = false;
  @Input() modalId = 0;
  @Input() id = 0;
  @Input() card?: CardModel;
  @Output() openChange = new EventEmitter<number>();
  @Output() editedComponent = new EventEmitter();
  
  readonly facade: EditCardDialogFacade;

  constructor() {
    super();
    this.facade = inject(EditCardDialogFacade);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['open']?.currentValue === true) {
      this.facade.reset(this.card ?? undefined);
    }
  }

  onSubmit(): void {
    const payload = this.facade.buildSubmitPayload(this.id);
    this.close();
    this.editedComponent.emit(payload);
  }
}
