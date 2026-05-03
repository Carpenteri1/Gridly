import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ModalDirective } from '../../../Directives/modal.directive';
import { BaseModalComponent } from '../../../Directives/base-modal.component';
import { WidgetType } from '../../../Types/widget.type.enum';
import { WidgetOptionsModal } from '../../../Models/WidgetOptionsModal';
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
  @Input() widgetOptions: WidgetOptionsModal[] = [];

  @Output() newWidget = new EventEmitter<CardModel>();

  onSelect(type: WidgetType) {
    switch (type) {
      case WidgetType.Custom:
        return this.newWidget.emit(new CardModel());
      default:
        return this.newWidget.emit(new CardModel());
    }
  }
}
