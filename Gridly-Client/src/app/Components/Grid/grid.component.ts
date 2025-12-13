import {AfterViewChecked, Component, ElementRef, OnInit, Renderer2} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ComponentService} from "../../Services/component.service";
import {ModalViewModel} from "../../Models/ModalView.Model";
import {ComponentEndPointType} from "../../Types/endPoint.type.enum";
import {ItemComponent} from "../Item/item.component";
import {CdkDrag, CdkDragDrop, CdkDropList, moveItemInArray} from "@angular/cdk/drag-drop";
import {ResizableDirective} from "../../Directives/resizable.directive";
import {ComponentModel} from "../../Models/Component.Model";
import {EditWidgetDialogComponent} from "../DialogComponents/EditWidgetDialog/edit-widget-dialog.component";
import { DialogService } from '../../Services/dialog.service';
import { SetModalComponentFormData } from '../../Utils/viewModel.factory';
import {ModalFormType} from "../../Types/modalForm.types.enum";

@Component({
  selector: 'grid-component',
  imports: [CommonModule, ItemComponent, CdkDropList, CdkDrag, EditWidgetDialogComponent],
  templateUrl: './grid.component.html',
  standalone: true,
  styleUrls: ['./grid.component.css'],
})

export class GridComponent implements AfterViewChecked, OnInit{
  protected modalModel!: ModalViewModel;
  protected readonly FormType = ModalFormType;

  constructor(
    protected componentService: ComponentService,
    protected modalService: DialogService,
    private render: Renderer2,
    private el: ElementRef) {
  }
  
  protected handleSelect(t: string) {
    this.modalService.Submit(SetModalComponentFormData({type: this.FormType.Edit}));
  }

  async ngOnInit(): Promise<void> {
    if(this.componentService.Components === undefined){
      this.componentService.Components = await this.componentService.CallEndpoint(ComponentEndPointType.Get) as ComponentModel[];
    }
  }

  ngAfterViewChecked() {
    if(!this.componentService.InAnyMode &&
      this.componentService.Components !== undefined)
    {
      this.SetLayout();
    }
  }

  protected Drop(event: CdkDragDrop<any[]>): void {
    if (!this.componentService.InDragMode) return;
    moveItemInArray(this.componentService.Components, event.previousIndex, event.currentIndex);
  }

  protected SetLayout() {
    for (const item of this.componentService.Components) {
      let el = document.getElementById(item.id.toString()) == null ? this.el.nativeElement : document.getElementById(item.id.toString());
      this.render.setStyle(el, 'height', item.componentSettings?.height + 'px');
      this.render.setStyle(el, 'flex', '0 0 ' + item.componentSettings?.width + 'px');
    }
  }
}
