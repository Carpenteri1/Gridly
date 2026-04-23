import {Component, inject, OnInit, signal} from "@angular/core";
import {TextStringsUtil} from "../../Constants/text.strings.util";
import {CommonModule} from "@angular/common";
import {VersionService} from "../../Services/version.service";
import {AddWidgetModalComponent} from "../ModalComponents/AddWidgetModal/add-widget-modal.component";
import { WidgetType } from "../../Types/widget.type.enum";
import { ComponentModel } from "../../Models/Component.Model";
import { ComponentService } from "../../Services/component.service";

@Component({
  selector: 'header-component',
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
  standalone: true,
  imports: [CommonModule, AddWidgetModalComponent]
})
export class HeaderComponent {

  #componentService = inject(ComponentService);
  #versionService = inject(VersionService);

  component$ = this.#componentService.component$;
  version$ = this.#versionService.version$;

  showMenu = signal(false);

  protected readonly TextStringsUtil = TextStringsUtil;
  protected addWidgetDialogActive = false;
  
  protected widgetOptions = [
    { type: WidgetType.Empty, label: 'Add empty widget', description: '', icon: 'bi bi-box' },
    { type: WidgetType.Custom, label: 'Add custom widget', description: '', icon: 'bi bi-box-fill' },
  ]


  /* TODO variants later maybe

  widgetOptions = [
    { type: 'chart', label: 'Chart', description: 'Visualize trends', icon: 'bi bi-graph-up' },
    { type: 'table', label: 'Table', description: 'Tabular data', icon: 'bi bi-table' },
    { type: 'kpi',   label: 'KPI',   description: 'Single metric',   icon: 'bi bi-speedometer2' },
    { type: 'note',  label: 'Note',  description: 'Plain text note', icon: 'bi bi-sticky' }
  ];*/

  protected add(component: ComponentModel) {
    this.#componentService.add(component);
  }

  //componentService.EditComponentsData(componentService.Components)
  //TODO get all components and activate edit mode
  protected setEditMode(){
    
  }

  toggleMenu(): void {
    this.showMenu.update((showMenu) => showMenu);
  }
}
