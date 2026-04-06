import { Component, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ModalDirective } from '../../../Directives/modal.directive';
import { BaseModalComponent } from '../../../Directives/base-modal.component';
import { ModalService } from '../../../Services/modal.service';
import { ComponentModel } from '../../../Models/Component.Model';
import { ModalType } from '../../../Types/modaltypes.enum';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { EditWidgetModalFacade } from './edit-widget-modal.facade';

@Component({
  selector: 'edit-widget-modal',
  imports: [CommonModule, FormsModule, ModalDirective, MatIconModule, MatSelectModule, MatInputModule],  
  templateUrl: './edit-widget-modal.component.html',
  styleUrls: ['../../../css/shared.modal.css', './edit-widget-modal.component.css'],
  providers: [EditWidgetModalFacade],
  standalone: true
})
export class EditWidgetModalComponent extends BaseModalComponent implements OnChanges {
  @Input() open: boolean = false;
  @Input() modalId: number = 0;
  @Input() id: number = 0;
  @Input() component?: ComponentModel;
  @Output() openChange = new EventEmitter<number>();
  @Output() editedComponent = new EventEmitter();
  
  readonly facade: EditWidgetModalFacade;

  constructor(modalService: ModalService, facade: EditWidgetModalFacade) {
    super(modalService);
    this.facade = facade;
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['open'] && this.modalDirective) {
      this.modalDirective.open = this.open;
    }
    if (changes['open']?.currentValue === true) {
      this.facade.reset(this.component ?? undefined);
    }
    if (changes['modalId'] && this.modalDirective) {
      this.modalDirective.modalId = this.modalId;
    }
    if (changes['id'] && this.modalDirective) {
      this.modalDirective.id = this.id;
    }
  }

  ngAfterViewInit(): void {
    if (this.modalDirective) {
      this.modalDirective.openChange.subscribe((modalId) => {
        this.openChange.emit(modalId);
      });
    }
  }

  onSubmit() {
    const payload = this.facade.buildSubmitPayload(this.id);
    this.close();
    this.editedComponent.emit(payload);
  }
}
