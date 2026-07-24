import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { DialogDirective } from '../../../directives/dialog.directive';
import { BaseDialogComponent } from '../../../directives/base-dialog.directive';
import { CardTypes } from '../../../types/card.types.enum';
import { CardOptionModel } from '../../../models/cardOptions.Model';
import { CardModel } from '../../../models/card.Model';
import { DialogService } from '../../../services/dialog_services/dialog.service';
import {WidgetService} from "../../../services/widget_services/widget.service";
import {AsyncPipe} from "@angular/common";
import {Widget} from "../../../interfaces/widget.Interface";
import {MatIcon} from "@angular/material/icon";

@Component({
  selector: 'app-add-card-dialog',
  standalone: true,
  imports: [DialogDirective, AsyncPipe, MatIcon],
  templateUrl: './add-card-dialog.component.html',
  styleUrls: ['./add-card-dialog.component.css'],
})
export class AddCardDialogComponent
  extends BaseDialogComponent
{
  @Input() open = false;
  @Input() cardOptions: CardOptionModel[] = [];
  @Output() newCard = new EventEmitter<CardModel>();

  #dialogService = inject(DialogService);
  #widgetService = inject(WidgetService);
  widgetOptions$ = this.#widgetService.get();

  onSelect(widget: Widget) {
    const card = new CardModel();
    card.name = widget.label;
    card.settings = this.#dialogService.settings();
    card.iconData = this.#dialogService.icon(widget.icon);

    switch (widget.widgetType) {
      case CardTypes.Empty:
        return this.newCard.emit(card);
        case CardTypes.Weather:
          return this.newCard.emit(card);
      default:
        return this.newCard.emit(card);
    }
  }
}
