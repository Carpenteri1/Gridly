import {Component, computed, inject, Input} from '@angular/core';
import { CommonModule } from '@angular/common';
import { TextStringsUtil } from '../../constants/text.strings.util';
import { CardModel } from '../../models/card.Model';
import { CdkDragHandle } from '@angular/cdk/drag-drop';
import { EditCardDialogComponent } from '../dialogs/editCardDialog/edit-card-dialog.component';
import { DeleteCardDialogComponent } from '../dialogs/deleteCardDialog/delete-card-dialog.component';
import { ResizableDirective } from '../../directives/resizable.directive';
import { MatIconModule } from '@angular/material/icon';
import { GridService } from '../../services/grid_services/grid.service';
import { CardRulesService } from '../../services/card_services/card-rules.service';
import {DialogService} from "../../services/dialog_services/dialog.service";
import {ProviderKeyDialogComponent} from "../dialogs/apiKeyDialog/provider-key-dialog.component";
import {ProviderKeysService} from "../../services/provider_key_services/provider-keys.service";
import {CardTypes} from "../../enums/card.types.enum";
import {ProviderKeyStatus} from "../../enums/provider-key-status.enum";

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
    MatIconModule,
    ProviderKeyDialogComponent
  ],
})
export class CardComponent {
  @Input({ required: true }) card!: CardModel;

  #gridService = inject(GridService);
  #cardRulesService = inject(CardRulesService);
  #dialogService = inject(DialogService);
  #providerKeyService = inject(ProviderKeysService)

  isAddProviderDialogOpen = this.#dialogService.isAddProviderDialogOpen
  isDeleteDialogOpen = this.#dialogService.isDeleteDialogOpen;
  isEditDialogOpen = this.#dialogService.isEditDialogOpen;

  providerKeyStatus = this.#providerKeyService.currentStatus;

  editActive = this.#gridService.inEditMode;

  showProviderKeyButton = computed(() => {
    if (this.card.type !== CardTypes.Weather) return false;
    const status = this.providerKeyStatus();
    return !(status?.exists && status.keyStatus === ProviderKeyStatus.Valid);
  });

  handleDialogChange(dialogId: number): void {
    if (dialogId === this.card.id) {
      this.#dialogService.closeAddProviderKeyDialog();
      this.#dialogService.closeDeleteDialog();
      this.#dialogService.closeEditDialog();
    }
  }

  isProviderDialogOpen = computed(() =>
    this.isAddProviderDialogOpen() === this.card.id
  );
  isEditDialogOpenForCard = computed(() =>
    this.isEditDialogOpen() === this.card.id
  );
  isDeleteDialogOpenForCard = computed(() =>
    this.isDeleteDialogOpen() === this.card.id
  );

  protected edit(card: CardModel): void {
    this.#gridService.updateCardInView(this.card, card);
  }

  protected remove(card: CardModel): void {
    this.#gridService.removeCardFromView(card);
  }

  protected hasMaterialIcon(item: CardModel): boolean {
    return this.#cardRulesService.hasMaterialIcon(item);
  }

  async openEditDialog() {
    this.#dialogService.openEditDialog(this.card.id);
  }

  openDeleteDialog(): void {
    this.#dialogService.openDeleteDialog(this.card.id);
  }

  openAddProviderKeyDialog(): void {
    this.#dialogService.openProviderKeyDialog(this.card.id);
  }

  protected readonly TextStringsUtil = TextStringsUtil;
}
