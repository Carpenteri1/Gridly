import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ModalDirective } from '../../../Directives/modal.directive';
import { BaseModalComponent } from '../../../Directives/base-modal.component';
import { WidgetType } from '../../../Types/widget.type.enum';
import { WidgetOptionsModal } from '../../../Models/WidgetOptionsModal';
import { CardModel } from '../../../Models/Card.Model';

@Component({
  selector: 'app-add-widget-modal',
  standalone: true,
  imports: [ModalDirective],
  templateUrl: './add-widget-modal.component.html',
  styleUrls: ['./add-widget-modal.component.css'],
})
export class AddWidgetModalComponent
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
