import {Component, Input} from "@angular/core";
import {ComponentService} from "../../Services/component.service";
import {SetModalComponentFormData} from "../../Utils/viewModel.factory";
import {DialogService} from "../../Services/dialog.service";
import {TextStringsUtil} from "../../Constants/text.strings.util";
import {ComponentModel} from "../../Models/Component.Model";
import {ModalFormType} from "../../Types/modalForm.types.enum";
import {NgIf} from "@angular/common";
import {EditWidgetDialogComponent} from "../DialogComponents/EditWidgetDialog/edit-widget-dialog.component";

@Component({
  selector: 'item-component',
  templateUrl: './item.component.html',
  styleUrl: './item.component.css',
  standalone: true,
  imports: [NgIf, EditWidgetDialogComponent]
})

export class ItemComponent {
  @Input() id!: number;
  protected open = false;
  constructor(public componentService: ComponentService, public modalService: DialogService) {}

  protected IconFilePath(item: ComponentModel): string {
    return "Assets/Icons/" + item.iconData?.name + "." + item.iconData?.type;
  }

  get CurrentComponentId():number {
    this.componentService.Component = this.componentService.GetComponentById(this.id)!;
    return this.componentService.Component.id;
  }

  protected handleSelect(t: string) {
    this.modalService.Submit(SetModalComponentFormData({type: this.FormType.Edit}));
  }

  protected readonly TextStringsUtil = TextStringsUtil;
  protected readonly SetModalComponentFormData = SetModalComponentFormData;
  protected readonly FormType = ModalFormType;
  protected readonly Component = Component;
}
