import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ModalDirective } from '../../../Directives/modal.directive';
import { BaseModalComponent } from '../../../Directives/base-modal.component';
import { CardTypes } from '../../../Types/card.types.enum';
import { CardOptionModel } from '../../../Models/WidgetOptionsModal';
import { CardModel } from '../../../Models/card.Model';

@Component({
  selector: 'app-add-card-dialog',
  standalone: true,
  imports: [ModalDirective],
  templateUrl: './add-card-dialog.component.html',
  styleUrls: ['./add-card-dialog.component.css'],
})
export class AddCardDialogComponent
  extends BaseModalComponent
{
  @Input() open = false;
  @Input() cardOptions: CardOptionModel[] = [];

  @Output() newCard = new EventEmitter<CardModel>();

  onSelect(type: CardTypes) {
    switch (type) {
      case CardTypes.Custom:
        return this.newCard.emit(new CardModel());
      default:
        return this.newCard.emit(new CardModel());
    }
  }
}
