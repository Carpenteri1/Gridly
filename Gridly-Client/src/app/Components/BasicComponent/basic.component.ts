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
import {IconModel} from "../../Models/Icon.Model";
import {EndPointType} from "../../Types/endPoint.type.enum";

@Component({
  selector: 'basic-component',
  imports: [CommonModule, CdkDrag, CdkDropList, ResizableDirective, MatButton, MatTooltip],
  templateUrl: './basic.component.html',
  standalone: true,
  styleUrls: ['./basic.component.css']
})

export class BasicComponent implements AfterViewChecked {
  protected resizableActive!: boolean;
  protected modalModel!: ModalViewModel;

  constructor(
    protected componentService: ComponentService,
    protected modalService: ModalService,
    private render: Renderer2,
    private el: ElementRef) {
    componentService.CallEndpoint(EndPointType.Get)
  }

  ngAfterViewChecked() {
    this.SetComponentLayout();
  }

  SetComponentLayout() {
    for (let index in this.componentService.components) {
      let item = this.componentService.components[index];
      let el = document.getElementById(item.id.toString()) == null ? this.el.nativeElement : document.getElementById(item.id.toString());
      this.render.setStyle(el, 'height', item.componentSettings?.height + 'px');
      this.render.setStyle(el, 'flex', '0 0 ' + item.componentSettings?.width + 'px');
    }
  }

   ActivateResize(item: any): void {
    this.resizableActive = true;
    this.componentService.component = item;
   }

    DisableResize(item: any): void {
      if(this.resizableActive){
        this.resizableActive = false;
      }
    }

  HaveIconSet(iconData: IconModel | undefined):boolean{
    if(iconData === undefined){
      return false;
    }
    return iconData.name !== undefined && iconData.name !== "";
  }

  HaveImagUrlSet(imageUrl:string | undefined):boolean{
    return imageUrl !== undefined && imageUrl != null && imageUrl !== "";
  }

  IconFilePath(name:string, fileType:string): string {
    return "Assets/Icons/" + name + "." + fileType;
  }

  Drop(event: CdkDragDrop<any[]>): void {
    if (!this.componentService.dragMode) return;
    moveItemInArray(this.componentService.components, event.previousIndex, event.currentIndex);
  }

  protected readonly FormType = ModalFormType;
  protected readonly SetModalComponentFormData = SetModalComponentFormData;
  protected readonly TextStringsUtil = TextStringsUtil;
}
