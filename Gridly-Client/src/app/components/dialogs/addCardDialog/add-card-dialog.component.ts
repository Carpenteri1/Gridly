import { Component, EventEmitter, inject, Input, Output, signal } from '@angular/core';
import { DialogDirective } from '../../../directives/dialog.directive';
import { BaseDialogComponent } from '../../../directives/base-dialog.directive';
import { CardTypes } from '../../../enums/card.types.enum';
import { CardOptionModel } from '../../../models/cardOptions.Model';
import { CardModel } from '../../../models/card.Model';
import { DialogService } from '../../../services/dialog_services/dialog.service';
import {WidgetService} from "../../../services/widget_services/widget.service";
import {Widget} from "../../../interfaces/widget.Interface";
import {MatIcon} from "@angular/material/icon";
import {AsyncPipe} from "@angular/common";
import {FormsModule} from "@angular/forms";

@Component({
  selector: 'app-add-card-dialog',
  standalone: true,
  imports: [DialogDirective, MatIcon, AsyncPipe, FormsModule],
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

  protected readonly CardTypes = CardTypes;
  protected readonly timeZoneOptions = Intl.supportedValuesOf('timeZone');

  protected selectedWidget = signal<Widget | null>(null);
  protected selectedTimeZone = signal(Intl.DateTimeFormat().resolvedOptions().timeZone);
  protected selectedDisplayFormat = signal<'digital' | 'analog'>('digital');

  onSelect(widget: Widget) {
    if (widget.widgetType === CardTypes.Clock) {
      this.selectedWidget.set(widget);
      return;
    }

    this.emitCard(widget);
  }

  confirmClock(): void {
    const widget = this.selectedWidget();
    if (!widget) return;

    this.emitCard(widget);
    this.selectedWidget.set(null);
  }

  cancelClockConfig(): void {
    this.selectedWidget.set(null);
  }

  private emitCard(widget: Widget): void {
    const card = new CardModel();
    card.name = widget.label;
    card.type = widget.widgetType;
    card.settings = this.#dialogService.settings();
    card.iconData = this.#dialogService.setIcon(widget.icon);

    switch (widget.widgetType) {
      case CardTypes.Clock:
        card.settings.timeZone = this.selectedTimeZone();
        card.settings.displayFormat = this.selectedDisplayFormat();
        return this.newCard.emit(card);
      case CardTypes.Empty:
        return this.newCard.emit(card);
      case CardTypes.Weather:
        return this.newCard.emit(card);
      default:
        return this.newCard.emit(card);
    }
  }
}
