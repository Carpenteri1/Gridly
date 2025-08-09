import {AfterViewChecked, Component, ElementRef, Renderer2} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ResizableDirective} from "../../Directives/resizable.directive";
import {CdkDrag, CdkDragDrop, CdkDropList, moveItemInArray} from '@angular/cdk/drag-drop';
import {ModalFormType} from "../../Types/modalForm.types.enum";
import {ComponentService} from "../../Services/component.service";
import {ModalService} from "../../Services/modal.service";
import {ModalViewModel} from "../../Models/ModalView.Model";
import {SetModalComponentFormData} from "../../Utils/viewModel.factory";
import {TextStringsUtil} from "../../Constants/text.strings.util";
import {MatButton} from "@angular/material/button";
import {MatTooltip} from "@angular/material/tooltip";
import {EndPointType} from "../../Types/endPoint.type.enum";
import {ComponentModel} from "../../Models/Component.Model";

@Component({
  selector: 'grid-component',
  imports: [CommonModule, CdkDrag, CdkDropList, ResizableDirective, MatButton, MatTooltip],
  templateUrl: './grid.component.html',
  standalone: true,
  styleUrls: ['./grid.component.css']
})

export class GridComponent implements AfterViewChecked {
  protected modalModel!: ModalViewModel;

  constructor(
    protected componentService: ComponentService,
    protected modalService: ModalService,
    private render: Renderer2,
    private el: ElementRef) {
    componentService.CallEndpoint(EndPointType.Get)
  }

  ngAfterViewChecked() {
    this.SetLayout();
  }

  SetLayout() {
    for (let index in this.componentService.GetAllComponents) {
      let item = this.componentService.GetAllComponents[index];
      let el = document.getElementById(item.id.toString()) == null ? this.el.nativeElement : document.getElementById(item.id.toString());
      this.render.setStyle(el, 'height', item.componentSettings?.height + 'px');
      this.render.setStyle(el, 'flex', '0 0 ' + item.componentSettings?.width + 'px');
    }
  }

  IconFilePath(item: ComponentModel): string {
      return "Assets/Icons/" + item.iconData?.name + "." + item.iconData?.type;
  }

  Drop(event: CdkDragDrop<any[]>): void {
    if (!this.componentService.InDragMode) return;
    moveItemInArray(this.componentService.GetAllComponents, event.previousIndex, event.currentIndex);
  }

  protected readonly FormType = ModalFormType;
  protected readonly SetModalComponentFormData = SetModalComponentFormData;
  protected readonly TextStringsUtil = TextStringsUtil;
}
