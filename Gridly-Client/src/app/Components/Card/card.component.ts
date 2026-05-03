import { Component, inject, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ComponentService } from '../../services/component.service';
import { ComponentRulesService } from '../../services/component-rules.service';
import { TextStringsUtil } from '../../constants/text.strings.util';
import { CardModel } from '../../models/card.Model';
import { CdkDragHandle } from '@angular/cdk/drag-drop';
import { DeleteCardDialogComponent } from '../dialog/deleteCardDialog/delete-card-dialog.component';
import { EditCardDialogComponent } from '../dialog/editCardDialog/edit-card-dialog.component';
import { ResizableDirective } from '../../directives/resizable.directive';
import { MatIconModule } from '@angular/material/icon';
import { GridService } from '../../services/grid.service';

@Component({
  selector: 'app-card-component',
  templateUrl: './card.component.html',
  styleUrl: './card.component.css',
  standalone: true,
  imports: [
    CommonModule,
    CdkDragHandle,
    DeleteCardDialogComponent,
    EditCardDialogComponent,
    ResizableDirective,
    MatIconModule
  ],
})
export class CardComponent {
  @Input({ required: true }) card!: CardModel;

  #componentService = inject(ComponentService);
  #componentRulesService = inject(ComponentRulesService);
  #gridService = inject(GridService);

  isEditModalOpen = false;
  isDeleteModalOpen = false;
  readonly inEditMode = this.#gridService.editMode;

  handleModalChange(modalId: number): void {
    if (modalId === this.card.id) {
      this.isEditModalOpen = false;
      this.isDeleteModalOpen = false;
    }
  }

  protected edit(card: CardModel): void {
    void this.#componentService.edit(card);
  }

  protected remove(id: number): void {
    void this.#componentService.delete(id);
  }

  protected hasMaterialIcon(card: CardModel): boolean {
    return this.#componentRulesService.hasMaterialIcon(card);
  }

  openEditDialog(): void {
    this.isEditModalOpen = true;
  }

  openDeleteDialog(): void {
    this.isDeleteModalOpen = true;
  }

  protected readonly TextStringsUtil = TextStringsUtil;
}
