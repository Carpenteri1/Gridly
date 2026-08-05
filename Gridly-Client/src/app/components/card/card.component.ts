import {Component, computed, inject, Input} from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
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
import {SetLocationForProviderDialogComponent} from "../dialogs/setLocationForProviderDialog/set-location-for-provider-dialog.component";

@Component({
  selector: 'app-card-component',
  templateUrl: './card.component.html',
  styleUrl: './card.component.css',
  standalone: true,
  imports: [
    CommonModule,
    TranslatePipe,
    CdkDragHandle,
    EditCardDialogComponent,
    DeleteCardDialogComponent,
    ResizableDirective,
    MatIconModule,
    ProviderKeyDialogComponent,
    SetLocationForProviderDialogComponent
  ],
})
export class CardComponent {
  @Input({ required: true }) card!: CardModel;

  #gridService = inject(GridService);
  #cardRulesService = inject(CardRulesService);
  #dialogService = inject(DialogService);
  #providerKeyService = inject(ProviderKeysService)

  private _isAddProviderKeyDialogOpen = this.#dialogService.isAddProviderDialogOpen
  private _isSetProviderLocationDialogOpen = this.#dialogService.isSetProviderLocationDialogOpen
  private _isDeleteDialogOpen = this.#dialogService.isDeleteDialogOpen;
  private _isEditDialogOpen = this.#dialogService.isEditDialogOpen;

  providerKeyStatus = this.#providerKeyService.currentStatus;

  editActive = this.#gridService.inEditMode;

  showProviderKeyButton = computed(() => {
    if (this.card.type !== CardTypes.Weather) return false;
    const status = this.providerKeyStatus();
    return !(status?.exists && status.keyStatus === ProviderKeyStatus.Valid);
  });

  handleDialogChange(dialogId: number): void {
    if (dialogId === this.card.id) {
      this.#dialogService.closeDeleteDialog();
      this.#dialogService.closeEditDialog();
    }
  }

  isAddProviderKeyDialogOpen = computed(() =>
    this._isAddProviderKeyDialogOpen() === this.card.id
  );
  isSetProviderLocationDialogOpen = computed(() =>
    this._isSetProviderLocationDialogOpen() === this.card.id
  );
  isEditDialogOpenForCard = computed(() =>
    this._isEditDialogOpen() === this.card.id
  );
  isDeleteDialogOpenForCard = computed(() =>
    this._isDeleteDialogOpen() === this.card.id
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

  protected SaveProviderKey(dialogId: number) {
    if (dialogId === this.card.id) {
      this.#dialogService.closeAddProviderKeyDialog();
    }
    if(this.#dialogService.isAddProviderDialogOpen() === null) {
      this.#dialogService.openSetProviderLocationDialog(this.card.id);
    }
  }

  protected SaveProviderLocation(dialogId: number) {
    if (dialogId === this.card.id) {
      this.#dialogService.closeSetProviderLocationDialog();
    }
  }
}
