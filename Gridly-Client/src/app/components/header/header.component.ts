import {Component, computed, inject} from "@angular/core";
import { LocaleStringsService } from "../../services/locale_services/locale-strings.service";
import { CommonModule } from "@angular/common";
import { VersionService } from "../../services/version_services/version.service";
import { AddCardDialogComponent } from "../dialogs/addCardDialog/add-card-dialog.component";
import { CardModel } from "../../models/card.Model";
import { GridService } from "../../services/grid_services/grid.service";
import {ProviderKeyDialogComponent} from "../dialogs/apiKeyDialog/provider-key-dialog.component";
import {DialogService} from "../../services/dialog_services/dialog.service";
import {
  SetLocationForProviderDialogComponent
} from "../dialogs/setLocationForProviderDialog/set-location-for-provider-dialog.component";

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
  standalone: true,
  imports: [CommonModule, AddCardDialogComponent, ProviderKeyDialogComponent, SetLocationForProviderDialogComponent]
})
export class HeaderComponent {

  #versionService = inject(VersionService);
  #gridService = inject(GridService);
  #dialogService = inject(DialogService);

  version$ = this.#versionService.version$;

  _isAddProviderDialogOpen = this.#dialogService.isAddProviderDialogOpen

  readonly locale = inject(LocaleStringsService).locale;
  addDialogActive = false;

  editActive = this.#gridService.inEditMode;

  protected add(card: CardModel): void {
    this.addDialogActive = false;
    this.#gridService.addCardToFirstAvailableRow(card);
  }

  toggleMenu(): void {
    this.#gridService.toggleEdit();
    if (!this.#gridService.inEditMode()) {
      this.reloadPage();
    }
  }

  isAddProviderDialogOpen = computed(() =>
    this._isAddProviderDialogOpen() === 0
  );

  openAddProviderKeyDialog(): void {
    this.#dialogService.openProviderKeyDialog(0);
  }

  async save(): Promise<void> {
    await this.#gridService.batchSave(this.#gridService.currentRowColumns());
    this.toggleMenu();
  }

  protected reloadPage(): void {
    location.reload();
  }

  protected handleDialogChange(dialogId: number): void {
    if (dialogId === 0) {
      this.#dialogService.closeAddProviderKeyDialog();
    }
  }
}
