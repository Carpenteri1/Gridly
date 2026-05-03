import { Component, inject, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardService } from '../../services/card.service';
import { TextStringsUtil } from '../../constants/text.strings.util';
import { CardModel } from '../../models/card.Model';
import { CdkDragHandle } from '@angular/cdk/drag-drop';
import { EditCardDialogComponent } from '../dialogs/editCardDialog/edit-card-dialog.component';
import { DeleteCardDialogComponent } from '../dialogs/deleteCardDialog/delete-card-dialog.component';
import { ResizableDirective } from '../../directives/resizable.directive';
import { MatIconModule } from '@angular/material/icon';
import { GridService } from '../../services/grid.service';
import { ComponentRulesService } from '../../services/card-rules.service';

@Component({
  selector: 'app-card-component',
  templateUrl: './card.component.html',
  styleUrl: './card.component.css',
  standalone: true,
  imports: [
    CommonModule,
    CdkDragHandle,
    EditCardDialogComponent,
    DeleteCardDialogComponent,
    ResizableDirective,
    MatIconModule
  ],
})
export class CardComponent {
  @Input({ required: true }) card!: CardModel;

  #cardService = inject(CardService);
  #gridService = inject(GridService);
  #componentRulesService = inject(ComponentRulesService);

  isEditDialogOpen = false;
  isDeleteDialogOpen = false;
  readonly inEditMode = this.#gridService.editMode;

  handleDialogChange(modalId: number): void {
    if (modalId === this.card.id) {
      this.isEditDialogOpen = false;
      this.isDeleteDialogOpen = false;
    }
  }

  protected edit(card: CardModel): void {
    void this.#cardService.edit(card);
  }

  protected remove(id: number): void {
    void this.#cardService.delete(id);
  }

  protected hasMaterialIcon(item: CardModel): boolean {
    return this.#componentRulesService.hasMaterialIcon(item);
  }

  openEditDialog(): void {
    this.isEditDialogOpen = true;
  }

  openDeleteDialog(): void {
    this.isDeleteDialogOpen = true;
  }

  protected readonly TextStringsUtil = TextStringsUtil;
}
