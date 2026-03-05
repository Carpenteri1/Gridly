import { Component, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ModalDirective } from '../../../Directives/modal.directive';
import { BaseModalComponent } from '../SharedModalComponents/base-modal.component';
import { ModalService } from '../../../Services/modal.service';
import { ComponentModel } from '../../../Models/Component.Model';
import { MapComponentData } from '../../../Utils/componentModel.factory';
import { ModalType } from '../../../Types/modaltypes.enum';

@Component({
  selector: 'prompt-modal',
  templateUrl: './prompt-modal.component.html',
  styleUrls: ['../../../css/shared.modal.css'],
  standalone: true,
  imports: [FormsModule, ModalDirective],
})
export class PromptModalComponent extends BaseModalComponent implements OnChanges {
  @Input() open: boolean = false;
  @Input() modalId: number = 0;
  @Input() id: number = 0;
  @Output() openChange = new EventEmitter<number>();
  @Output() deleteWidget = new EventEmitter<{component: ComponentModel, modalType: ModalType}>();

  constructor(modalService: ModalService) {
    super(modalService);
  }

  ngOnChanges(changes: SimpleChanges): void {
    setTimeout(() => {
      if (this.modalDirective) {
        if (changes['open']) {
          this.modalDirective.open = this.open;
        }
        if (changes['modalId']) {
          this.modalDirective.modalId = this.modalId;
        }
        if (changes['id']) {
          this.modalDirective.id = this.id;
        }
      }
    });
  }

  ngAfterViewInit(): void {
    if (this.modalDirective) {

      this.modalDirective.open = this.open;
      this.modalDirective.modalId = this.modalId;
      this.modalDirective.id = this.id;
      
      this.modalDirective.openChange.subscribe((modalId) => {
        this.openChange.emit(modalId);
      });
    }
  }

    onSubmit() {
      this.close();
      this.deleteWidget.emit({component: MapComponentData.Override({id: this.id}), modalType: ModalType.Delete});
    }
}
