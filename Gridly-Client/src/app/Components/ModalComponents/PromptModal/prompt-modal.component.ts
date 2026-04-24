import { Component, Input, Output, EventEmitter, OnChanges, SimpleChanges, AfterViewInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ModalDirective } from '../../../Directives/modal.directive';
import { BaseModalComponent } from '../../../Directives/base-modal.component';

@Component({
  selector: 'app-prompt-modal',
  templateUrl: './prompt-modal.component.html',
  styleUrls: ['../../../css/shared.modal.css'],
  standalone: true,
  imports: [FormsModule, ModalDirective],
})
export class PromptModalComponent extends BaseModalComponent implements OnChanges, AfterViewInit {
  @Input() open = false;
  @Input() modalId = 0;
  @Input() id = 0;
  @Output() openChange = new EventEmitter<number>();
  @Output() remove = new EventEmitter<{id: number}>();

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

  onSubmit(): void {
    this.close();
    this.remove.emit({ id: this.id });
  }
}
