import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { DialogDirective } from '../../../directives/dialog.directive';
import { BaseModalComponent } from '../../../directives/base-modal.component';
import { CardTypes } from '../../../types/card.types.enum';
import { CardOptionModel } from '../../../models/cardOptions.Model';
import { CardModel } from '../../../models/card.Model';
import { DialogService } from '../../../services/dialog_services/dialog.service';

@Component({
  selector: 'app-add-card-dialog',
  standalone: true,
  imports: [DialogDirective, DialogDirective],
  templateUrl: './add-card-dialog.component.html',
  styleUrls: ['./add-card-dialog.component.css'],
})
export class AddCardDialogComponent
  extends BaseModalComponent
{
  #dialogService = inject(DialogService);

  @Input() open = false;
  @Input() cardOptions: CardOptionModel[] = [];

  @Output() newCard = new EventEmitter<CardModel>();

  onSelect(type: CardTypes) {
    const card = new CardModel();
    card.settings = this.#dialogService.settings();
    card.iconData = this.#dialogService.icon();

    switch (type) {
      case CardTypes.Custom:
        return this.newCard.emit(card);
      default:
        return this.newCard.emit(card);
    }
  }
}
