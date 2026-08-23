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
import { CardTypes } from '../../../enums/card.types.enum';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-edit-card-dialog',
  imports: [CommonModule, FormsModule, DialogDirective, MatIconModule, MatSelectModule, MatInputModule, TranslatePipe],
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

  readonly facade: EditCardDialogFacade;
  protected readonly CardTypes = CardTypes;

  constructor() {
    super();
    this.facade = inject(EditCardDialogFacade);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['open']?.currentValue === true) {
      this.facade.reset(this.card ?? undefined);
    }
  }

  async onSubmit(): Promise<void> {
    if (this.facade.isWeatherCard) {
      const locationSaved = await this.facade.saveLocation(this.id);
      if (!locationSaved) return;
    }

    const payload = this.facade.buildSubmitPayload(this.id);
    this.close();
    this.editCard.emit(payload);
  }
}
