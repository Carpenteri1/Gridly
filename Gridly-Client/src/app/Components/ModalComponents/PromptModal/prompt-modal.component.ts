import { Component, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ModalDirective } from '../../../Directives/modal.directive';
import { BaseModalComponent } from '../../../Directives/base-modal.component';
import { ModalService } from '../../../Services/modal.service';
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
  @Output() remove = new EventEmitter<{id: number}>();

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
      this.remove.emit({id: this.id});
    }
}
