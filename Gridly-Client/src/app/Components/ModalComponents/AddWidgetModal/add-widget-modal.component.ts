import {
  Component,
  Input,
  Output,
  EventEmitter,
  AfterViewInit,
  OnChanges,
  SimpleChanges,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ModalDirective } from '../../../Directives/modal.directive';
import { BaseModalComponent } from '../SharedModalComponents/base-modal.component';
import { ModalBehaviorService } from '../../../Services/modal-behavior.service';
import { WidgetType } from '../../../Types/widget.type.enum';
import { WidgetOptionsModal } from '../../../Models/WidgetOptionsModal';

@Component({
  selector: 'add-widget-modal',
  standalone: true,
  imports: [CommonModule, ModalDirective],
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
  @Output() select = new EventEmitter<WidgetType>();

  constructor(modalBehavior: ModalBehaviorService) {
    super(modalBehavior);
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
        this.select.emit(type); // TODO use this value
        break;
      default:
        this.select.emit(type);
        this.close();
        break;
    }
  }

  get SelectedType(): EventEmitter<WidgetType> {
    return this.select;
  }
}
