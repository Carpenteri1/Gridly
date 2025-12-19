import {Component, Input} from "@angular/core";
import {CommonModule} from "@angular/common";
import {ComponentService} from "../../Services/component.service";
import {SetModalComponentFormData} from "../../Utils/viewModel.factory";
import {TextStringsUtil} from "../../Constants/text.strings.util";
import {ComponentModel} from "../../Models/Component.Model";
import {ModalFormType} from "../../Types/modalForm.types.enum";
import {CdkDragHandle} from "@angular/cdk/drag-drop";
import { ModalService } from "../../Services/modal.service";
import { EditWidgetModalComponent } from "../ModalComponents/EditWidgetModal/edit-widget-modal.component";

@Component({
  selector: 'item-component',
  templateUrl: './item.component.html',
  styleUrl: './item.component.css',
  standalone: true,
  imports: [CommonModule, CdkDragHandle, EditWidgetModalComponent],
})

export class ItemComponent {
  @Input() id!: number;
  constructor(public componentService: ComponentService, public modalService: ModalService) {}

  protected IconFilePath(item: ComponentModel): string {
    return "Assets/Icons/" + item.iconData?.name + "." + item.iconData?.type;
  }

  get CurrentComponentId():number {
    this.componentService.Component = this.componentService.GetComponentById(this.id)!;
    return this.componentService.Component.id;
  }

  //TODO might remove
  protected handleSelect(t: string) {
    this.modalService.Submit(SetModalComponentFormData({type: this.FormType.Edit}));
  }

  protected readonly TextStringsUtil = TextStringsUtil;
  protected readonly SetModalComponentFormData = SetModalComponentFormData;
  protected readonly FormType = ModalFormType;
  protected readonly Component = Component;
}
