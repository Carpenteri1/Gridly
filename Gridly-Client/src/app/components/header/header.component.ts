import { Component, inject } from "@angular/core";
import { TextStringsUtil } from "../../constants/text.strings.util";
import { CommonModule } from "@angular/common";
import { VersionService } from "../../services/version_services/version.service";
import { AddCardDialogComponent } from "../dialogs/addCardDialog/add-card-dialog.component";
import { CardModel } from "../../models/card.Model";
import { GridService } from "../../services/grid_services/grid.service";

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
  standalone: true,
  imports: [CommonModule, AddCardDialogComponent]
})
export class HeaderComponent {

  #versionService = inject(VersionService);
  #gridService = inject(GridService);

  version$ = this.#versionService.version$;

  readonly TextStringsUtil = TextStringsUtil;
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

  save(): void {
    this.toggleMenu();
    this.#gridService.batchSave(this.#gridService.currentRowColumns());
  }

  protected reloadPage(): void {
    location.reload();
  }

}
