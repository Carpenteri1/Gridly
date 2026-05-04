import { Component, EventEmitter, inject, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BaseModalComponent } from '../../../directives/base-modal.component';
import { CardModel } from '../../../models/card.Model';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { EditCardDialogFacade } from './edit-card-dialog.facade';
import { DialogDirective } from '../../../directives/dialog.directive';

@Component({
  selector: 'app-edit-card-dialog',
  imports: [CommonModule, FormsModule, DialogDirective, MatIconModule, MatSelectModule, MatInputModule],  
  templateUrl: './edit-card-dialog.component.html',
  styleUrls: ['../../../css/shared.modal.css', './edit-card-dialog.component.css'],
  providers: [EditCardDialogFacade],
  standalone: true
})
export class EditCardDialogComponent extends BaseModalComponent implements OnChanges {
  @Input() open = false;
  @Input() cardId = 0;
  @Input() id = 0;
  @Input() card?: CardModel;
  @Output() openChange = new EventEmitter<number>();
  @Output() editCard = new EventEmitter();
  
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
    this.editCard.emit(payload);
  }
}
