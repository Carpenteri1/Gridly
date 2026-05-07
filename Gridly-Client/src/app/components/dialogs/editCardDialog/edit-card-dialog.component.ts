import { Component, EventEmitter, inject, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BaseDialogComponent } from '../../../directives/base-dialog.directive';
import { CardModel } from '../../../models/card.Model';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { EditCardDialogFacade } from './edit-card-dialog.facade';
import { DialogDirective } from '../../../directives/dialog.directive';
import { GridService } from '../../../services/grid_services/grid.service';

@Component({
  selector: 'app-edit-card-dialog',
  imports: [CommonModule, FormsModule, DialogDirective, MatIconModule, MatSelectModule, MatInputModule],  
  templateUrl: './edit-card-dialog.component.html',
  styleUrls: ['../../../css/shared.dialog.css', './edit-card-dialog.component.css'],
  providers: [EditCardDialogFacade],
  standalone: true
})
export class EditCardDialogComponent extends BaseDialogComponent implements OnChanges {
  @Input() open = false;
  @Input() cardId = 0;
  @Input() id = 0;
  @Input() card?: CardModel;
  @Output() openChange = new EventEmitter<number>();
  @Output() editCard = new EventEmitter();
  
  #gridService = inject(GridService);

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
    this.#gridService.setEditMode(false);
  }
}
