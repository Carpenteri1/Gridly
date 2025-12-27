import {Component, OnInit, signal} from "@angular/core";
import {TextStringsUtil} from "../../Constants/text.strings.util";
import {CommonModule} from "@angular/common";
import {VersionService} from "../../Services/version.services";
import {ComponentService} from "../../Services/component.service";
import {ModalFormType} from "../../Types/modalForm.types.enum";
import {AddWidgetModalComponent} from "../ModalComponents/AddWidgetModal/add-widget-modal.component";
import { WidgetType } from "../../Types/widget.type.enum";

@Component({
  selector: 'header-component',
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
  standalone: true,
  imports: [CommonModule, AddWidgetModalComponent]
})
export class HeaderComponent implements OnInit {

  protected readonly TextStringsUtil = TextStringsUtil;
  protected readonly FormType = ModalFormType;
  protected open = false;
  
  protected widgetOptions = [
    { type: WidgetType.Empty, label: 'Add empty widget', description: '', icon: 'bi bi-box' },
    { type: WidgetType.Custom, label: 'Add custom widget', description: '', icon: 'bi bi-box-fill' },
  ]
  
  constructor(
    protected versionService: VersionService,
    protected componentService: ComponentService) {
  }

  /* TODO variants later maybe

  widgetOptions = [
    { type: 'chart', label: 'Chart', description: 'Visualize trends', icon: 'bi bi-graph-up' },
    { type: 'table', label: 'Table', description: 'Tabular data', icon: 'bi bi-table' },
    { type: 'kpi',   label: 'KPI',   description: 'Single metric',   icon: 'bi bi-speedometer2' },
    { type: 'note',  label: 'Note',  description: 'Plain text note', icon: 'bi bi-sticky' }
  ];*/

  protected handleSelect(t: string) {
    //this.modalService.Submit(SetModalComponentFormData({type: this.FormType.Add}));
  }

  async ngOnInit() {
    await this.versionService.SetVersion();
  }

  showMenu = signal(false);
  toggleMenu(): void {
    this.showMenu.update((showMenu) => showMenu);
  }
}
