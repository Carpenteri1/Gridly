import { Component, inject, signal } from "@angular/core";
import { TextStringsUtil } from "../../constants/text.strings.util";
import { CommonModule } from "@angular/common";
import { VersionService } from "../../services/version_services/version.service";
import { AddCardDialogComponent } from "../dialogs/addCardDialog/add-card-dialog.component";
import { CardTypes } from "../../types/card.types.enum";
import { CardModel } from "../../models/card.Model";
import { CardService } from "../../services/card_services/card.service";
import { GridService } from "../../services/grid_services/grid.service";

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
  standalone: true,
  imports: [CommonModule, AddCardDialogComponent]
})
export class HeaderComponent {

  #cardService = inject(CardService);
  #versionService = inject(VersionService);
  #gridService = inject(GridService);

  version$ = this.#versionService.version$;

  showMenu = signal(false);

  protected readonly TextStringsUtil = TextStringsUtil;
  protected addDialogActive = false;
  //TODO move to dialog
  protected cardOptions = [
    { type: CardTypes.Empty, label: 'Add empty card', description: '', icon: 'bi bi-box' },
    { type: CardTypes.Custom, label: 'Add custom card', description: '', icon: 'bi bi-box-fill' },
  ];


  /* TODO variants later maybe

  cardOptions = [
    { type: 'chart', label: 'Chart', description: 'Visualize trends', icon: 'bi bi-graph-up' },
    { type: 'table', label: 'Table', description: 'Tabular data', icon: 'bi bi-table' },
    { type: 'kpi',   label: 'KPI',   description: 'Single metric',   icon: 'bi bi-speedometer2' },
    { type: 'note',  label: 'Note',  description: 'Plain text note', icon: 'bi bi-sticky' }
  ];*/

  protected async add(card: CardModel): Promise<void> {
    this.addDialogActive = !this.addDialogActive;
    await this.#cardService.add(card);
  }

  //componentService.EditComponentsData(componentService.Components)
  //TODO get all components and activate edit mode
  protected async setEditMode(): Promise<void> {
    const cards = this.#cardService.currentCards();
    if (cards !== undefined) {
      await this.#cardService.saveOrder(cards);
    }

    this.#gridService.setEditMode(false);
    this.showMenu.set(false);
  }

  toggleMenu(): void {
    const next = !this.showMenu();
    this.showMenu.set(next);
    this.#gridService.setEditMode(next);
  }
}
