import { Component, inject, Input } from '@angular/core';
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
  @Input({ required: true }) component!: ComponentModel;

  #componentService = inject(ComponentService);
  #gridService = inject(GridService);

  isEditModalOpen = false;
  isDeleteModalOpen = false;
  readonly inEditMode = this.#gridService.editMode;

  protected iconDataUrl(item: ComponentModel): string {
    return `data:image/${item.iconData?.type};base64,${item.iconData?.base64Data}`;
  }

  protected hasFileIcon(item: ComponentModel): boolean {
    return this.#componentService.IconDataSet(item);
  }

  protected hasIconUrl(item: ComponentModel): boolean {
    return this.#componentService.IconUrlSet(item);
  }

  protected hasMaterialIcon(item: ComponentModel): boolean {
    return this.#componentService.MaterialIconSet(item);
  }

  handleModalChange(modalId: number): void {
    if (modalId === this.component.id) {
      this.isEditModalOpen = false;
      this.isDeleteModalOpen = false;
    }
  }

  protected edit(component: ComponentModel): void {
    void this.#componentService.edit(component);
  }

  protected remove(id: number): void {
    void this.#componentService.delete(id);
  }

  openEditDialog(): void {
    this.isEditModalOpen = true;
  }

  openDeleteDialog(): void {
    this.isDeleteModalOpen = true;
  }

  protected readonly TextStringsUtil = TextStringsUtil;
}
