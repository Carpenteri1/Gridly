import { Component, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ModalDirective } from '../../../Directives/modal.directive';
import { BaseModalComponent } from '../SharedModalComponents/base-modal.component';
import { ModalBehaviorService } from '../../../Services/modal-behavior.service';

@Component({
  selector: 'edit-widget-modal',
  standalone: true,
  imports: [CommonModule, FormsModule, ModalDirective],
  templateUrl: './edit-widget-modal.component.html',
  styleUrls: ['./edit-widget-modal.component.css'],
})
export class EditWidgetModalComponent extends BaseModalComponent implements OnChanges {
  @Input() open: boolean = false;
  @Input() modalId: number = 0;
  @Input() id: number = 0;
  @Output() openChange = new EventEmitter<number>();

  constructor(modalBehavior: ModalBehaviorService) {
    super(modalBehavior);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['open'] && this.modalDirective) {
      this.modalDirective.open = this.open;
    }
    if (changes['modalId'] && this.modalDirective) {
      this.modalDirective.modalId = this.modalId;
    }
    if (changes['id'] && this.modalDirective) {
      this.modalDirective.id = this.id;
    }
  }

  override ngAfterViewInit(): void {
    super.ngAfterViewInit?.();
    if (this.modalDirective) {
      this.modalDirective.openChange.subscribe((modalId) => {
        this.openChange.emit(modalId);
      });
    }
  }

  onSave() {
    // Component-specific save logic can be added here
    this.close();
  }
}
