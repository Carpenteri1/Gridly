import { Component, Input, Output, EventEmitter, OnChanges, SimpleChanges, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ModalDirective } from '../../../Directives/modal.directive';
import { BaseModalComponent } from '../SharedModalComponents/base-modal.component';
import { ModalService } from '../../../Services/modal.service';
import { MapComponentData } from '../../../Utils/componentModel.factory';
import { ComponentModel } from '../../../Models/Component.Model';
import { ModalType } from '../../../Types/modaltypes.enum';
import { IconService } from '../../../Services/Icon.service';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'edit-widget-modal',
  imports: [CommonModule, FormsModule, ModalDirective, MatIconModule, MatSelectModule, MatInputModule],  
  templateUrl: './edit-widget-modal.component.html',
styleUrls: ['../../../css/shared.modal.css', './edit-widget-modal.component.css'],
})
export class EditWidgetModalComponent extends BaseModalComponent implements OnChanges {
  @Input() open: boolean = false;
  @Input() modalId: number = 0;
  @Input() id: number = 0;
  @Output() openChange = new EventEmitter<number>();
  @Output() editWidget = new EventEmitter<{component: ComponentModel, modalType: ModalType}>();
  componentData: ComponentModel = MapComponentData();
  selectedOption: string = '';

  #iconService = inject(IconService);
  icons$ = this.#iconService.icons$;
  input$ = this.#iconService.searchInput$;

  constructor(modalService: ModalService) {
    super(modalService);
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

  ngAfterViewInit(): void {
    if (this.modalDirective) {
      this.modalDirective.openChange.subscribe((modalId) => {
        this.openChange.emit(modalId);
      });
    }
  }

  onSearch(input: string) {
    this.#iconService.searchInput$.next(input);
  }

  onSubmit() {
    this.componentData.id = this.id;
    this.close();
    this.editWidget.emit({component: MapComponentData(this.componentData), modalType: ModalType.Edit});
  }
}
