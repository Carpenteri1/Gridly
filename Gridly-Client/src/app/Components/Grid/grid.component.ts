import {AfterViewChecked, Component, ElementRef, OnInit, Renderer2} from '@angular/core';
import { CommonModule} from '@angular/common';
import { ComponentService } from "../../Services/component.service";
import { ModalViewModel } from "../../Models/ModalView.Model";
import { ItemComponent } from "../Item/item.component";
import { CdkDrag, CdkDragDrop, CdkDropList, moveItemInArray } from "@angular/cdk/drag-drop";
import { EditWidgetDialogComponent } from "../DialogComponents/EditWidgetDialog/edit-widget-dialog.component";
import { DialogService } from '../../Services/dialog.service';
import { SetModalComponentFormData } from '../../Utils/viewModel.factory';
import { ModalFormType } from "../../Types/modalForm.types.enum";
import { ComponentEndPointType } from '../../Types/endPoint.type.enum';
import { ComponentModel } from '../../Models/Component.Model';

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
    //TODO in testing mode add empty components
    if(this.componentService.Components === undefined){
      this.componentService.Components = await this.componentService.CallEndpoint(ComponentEndPointType.Get) as ComponentModel[];
    }
/*
    this.componentService.Components = [
      MapComponentData.Override({id: 1 ,name:"Title ett", iconUrl: "https://t4.ftcdn.net/jpg/16/18/52/61/360_F_1618526128_Kpdol855uNe6O7j4JFgMa4J9q9zBJLZb.jpg"}),
      MapComponentData.Override({id: 2 ,name:"Title två", iconUrl: ""}),
      MapComponentData.Override({id: 3 ,name:"Title tre", iconUrl: ""}),
      MapComponentData.Override({id: 4 ,name:"Title fyra", iconUrl: ""}),
      MapComponentData.Override({id: 5, name:"Title fem", iconUrl: ""})];*/
  }

  ngAfterViewChecked() {
    if(!this.componentService.InEditMode &&
      this.componentService.Components !== undefined)
    {
      this.SetLayout();
    }
  }

  protected Drop(event: CdkDragDrop<any[]>): void {
    if (!this.componentService.InEditMode) return;
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
