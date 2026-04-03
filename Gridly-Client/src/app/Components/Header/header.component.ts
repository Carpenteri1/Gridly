import {Component, inject, OnInit, signal} from "@angular/core";
import { take } from 'rxjs';
import {TextStringsUtil} from "../../Constants/text.strings.util";
import {CommonModule} from "@angular/common";
import {VersionService} from "../../Services/version.services";
import {AddWidgetModalComponent} from "../ModalComponents/AddWidgetModal/add-widget-modal.component";
import { WidgetType } from "../../Types/widget.type.enum";
import { ComponentModel } from "../../Models/Component.Model";
import { ComponentEndpointService } from "../../Services/endpoints/component.endpoint.service";

@Component({
  selector: 'header-component',
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
  standalone: true,
  imports: [CommonModule, AddWidgetModalComponent]
})
export class HeaderComponent implements OnInit {

  #componentEndpointService = inject(ComponentEndpointService);
  #versionEndpointService = inject(VersionService);
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

  protected handleSubmit(component: ComponentModel) {
    this.#componentEndpointService.add(component).pipe(take(1)).subscribe();
  }

  //componentService.EditComponentsData(componentService.Components)
  //TODO get all components and activate edit mode
  protected setEditMode(){
    
  }

  async ngOnInit() {
    await this.#versionEndpointService.SetVersion();
  }

  toggleMenu(): void {
    this.showMenu.update((showMenu) => showMenu);
  }
}
