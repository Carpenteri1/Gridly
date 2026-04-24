import { Component, inject, Input, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ComponentService } from '../../Services/component.service';
import { TextStringsUtil } from '../../Constants/text.strings.util';
import { ComponentModel } from '../../Models/Component.Model';
import { CdkDragHandle } from '@angular/cdk/drag-drop';
import { PromptModalComponent } from '../ModalComponents/PromptModal/prompt-modal.component';
import { EditWidgetModalComponent } from '../ModalComponents/EditWidgetModal/edit-widget-modal.component';
import { ResizableDirective } from '../../Directives/resizable.directive';
import { MatIconModule } from '@angular/material/icon';
import { GridService } from '../../Services/grid.service';

@Component({
  selector: 'app-item',
  templateUrl: './item.component.html',
  styleUrl: './item.component.css',
  standalone: true,
  imports: [
    CommonModule,
    CdkDragHandle,
    PromptModalComponent,
    EditWidgetModalComponent,
    ResizableDirective,
    MatIconModule
  ],
})
export class ItemComponent {
  @Input() id!: number; //TODO might not need this in the grid component

  @ViewChild(PromptModalComponent) promptModal!: PromptModalComponent;
  @ViewChild(EditWidgetModalComponent) editModal!: EditWidgetModalComponent;

  #componentService = inject(ComponentService);
  #gridService = inject(GridService);

  component$ = this.#componentService.component$;

  isEditModalOpen = false;
  isDeleteModalOpen = false;
  inEditMode = this.#gridService.getEditMode();

  protected IconFilePath(item: ComponentModel): string {
    return 'Assets/Icons/' + item.iconData?.name + '.' + item.iconData?.type;
  }
  handleModalChange(modalId: number): void {
    if (modalId === this.id) {
      this.isEditModalOpen = false;
      this.isDeleteModalOpen = false;
    }
  }

  protected edit(component: ComponentModel) {
    this.#componentService.edit(component);
  }

  protected remove(id: number) {
    this.#componentService.delete(id);
  }

  openEditDialog(): void {
    this.isEditModalOpen = true;
  }

  openDeleteDialog(): void {
    this.isDeleteModalOpen = true;
  }

  protected readonly TextStringsUtil = TextStringsUtil;
}
