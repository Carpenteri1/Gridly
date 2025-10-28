import {Component, OnInit, signal} from "@angular/core";
import {TextStringsUtil} from "../../Constants/text.strings.util";
import {CommonModule} from "@angular/common";
import {VersionService} from "../../Services/version.services";
import {DialogService} from "../../Services/dialog.service";
import {ComponentService} from "../../Services/component.service";
import {SetModalComponentFormData} from "../../Utils/viewModel.factory";
import {ModalFormType} from "../../Types/modalForm.types.enum";
import {AddWidgetDialogComponent} from "../DialogComponents/AddWidgetDialog/add-widget-dialog.component";

@Component({
  selector: 'header-component',
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
  standalone: true,
  imports: [CommonModule, AddWidgetDialogComponent]
})
export class HeaderComponent implements OnInit {
  constructor(
    protected versionService: VersionService,
    protected modalService: DialogService,
    protected componentService: ComponentService) {
  }
  protected open = false;
  protected widgetOptions = [
    { type: ModalFormType.Add, label: 'Add empty widget', description: '', icon: 'bi bi-box' },
  ]
  /* TODO variants later maybe

  widgetOptions = [
    { type: 'chart', label: 'Chart', description: 'Visualize trends', icon: 'bi bi-graph-up' },
    { type: 'table', label: 'Table', description: 'Tabular data', icon: 'bi bi-table' },
    { type: 'kpi',   label: 'KPI',   description: 'Single metric',   icon: 'bi bi-speedometer2' },
    { type: 'note',  label: 'Note',  description: 'Plain text note', icon: 'bi bi-sticky' }
  ];*/

  protected handleSelect(t: string) {
    this.modalService.Submit(SetModalComponentFormData({type: this.FormType.Add}));
  }

  async ngOnInit() {
    await this.versionService.SetVersion();
  }

  showMenu = signal(false);
  toggleMenu(): void {
    this.showMenu.update((showMenu) => showMenu);
  }

  protected readonly TextStringsUtil = TextStringsUtil;
  protected readonly FormType = ModalFormType;
}
