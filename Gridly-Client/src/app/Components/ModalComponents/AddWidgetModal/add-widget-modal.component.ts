import {
  Component,
  Input,
  Output,
  EventEmitter,
  AfterViewInit,
  OnChanges,
  SimpleChanges,
} from '@angular/core';
import { ModalDirective } from '../../../Directives/modal.directive';
import { BaseModalComponent } from '../../../Directives/base-modal.component';
import { ModalService } from '../../../Services/modal.service';
import { WidgetType } from '../../../Types/widget.type.enum';
import { WidgetOptionsModal } from '../../../Models/WidgetOptionsModal';
import { ComponentModel } from '../../../Models/Component.Model';

@Component({
  selector: 'add-widget-modal',
  standalone: true,
  imports: [ModalDirective],
  templateUrl: './add-widget-modal.component.html',
  styleUrls: ['./add-widget-modal.component.css'],
})
export class AddWidgetModalComponent
  extends BaseModalComponent
  implements AfterViewInit, OnChanges
{
  @Input() open: boolean = false;
  @Input() widgetOptions: WidgetOptionsModal[] = [];

  @Output() openChange = new EventEmitter<boolean>();
  @Output() newWidget = new EventEmitter<ComponentModel>();

  constructor(modalService: ModalService) {
    super(modalService);
  }

  ngAfterViewInit(): void {
    if (this.modalDirective) {
      this.modalDirective.openChange.subscribe(() => {
        this.openChange.emit(false);
      });
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['open'] && this.modalDirective) {
      this.modalDirective.open = this.open;
    }
  }

  handleOpenChange(modalId: number): void {
    this.openChange.emit(false);
  }

  onSelect(type: WidgetType) {
    switch (type) {
      case WidgetType.Custom:
        return this.newWidget.emit(new ComponentModel());
      default:
        return this.newWidget.emit(new ComponentModel());
    }
  }
}
