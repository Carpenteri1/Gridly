import { Component, inject, signal } from "@angular/core";
import { TextStringsUtil } from "../../Constants/text.strings.util";
import { CommonModule } from "@angular/common";
import { VersionService } from "../../Services/version.service";
import { AddCardDialogComponent } from "../Dialog/AddCardDialog/add-card-dialog.component";
import { CardTypes } from "../../Types/card.types.enum";
import { CardModel } from "../../Models/card.Model";
import { ComponentService } from "../../Services/component.service";
import { GridService } from "../../Services/grid.service";

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
  standalone: true,
  imports: [CommonModule, AddCardDialogComponent]
})
export class HeaderComponent {

  #componentService = inject(ComponentService);
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

  protected async add(component: CardModel): Promise<void> {
    this.addDialogActive = !this.addDialogActive;
    await this.#componentService.add(component);
  }

  //componentService.EditComponentsData(componentService.Components)
  //TODO get all components and activate edit mode
  protected setEditMode(): void {
    this.#gridService.toggle();
  }

  toggleMenu(): void {
    this.showMenu.update((showMenu) => !showMenu);
  }
}
