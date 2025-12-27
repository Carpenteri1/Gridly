import {AfterViewChecked, Component, ElementRef, OnInit, Renderer2} from '@angular/core';
import { CommonModule} from '@angular/common';
import { ComponentService } from "../../Services/component.service";
import { ModalViewModel } from "../../Models/ModalView.Model";
import { ItemComponent } from "../Item/item.component";
import { CdkDrag, CdkDragDrop, CdkDropList, moveItemInArray } from "@angular/cdk/drag-drop";
import { SetModalComponentFormData } from '../../Utils/viewModel.factory';
import { ModalFormType } from "../../Types/modalForm.types.enum";
import { MapComponentData } from '../../Utils/componentDialog.factory';
import { ComponentModel } from '../../Models/Component.Model';
import { ComponentEndPointType } from '../../Types/endPoint.type.enum';

@Component({
  selector: 'grid-component',
  imports: [CommonModule, CdkDropList, CdkDrag, ItemComponent],
  templateUrl: './grid.component.html',
  standalone: true,
  styleUrls: ['./grid.component.css'],
})

export class GridComponent implements AfterViewChecked, OnInit{
  protected modalModel!: ModalViewModel;
  protected readonly FormType = ModalFormType;

  constructor(
    protected componentService: ComponentService,
    private render: Renderer2,
    private el: ElementRef) {
  }
  /*
  protected handleSelect(t: string) {
    this.modalService.Submit(SetModalComponentFormData({type: this.FormType.Edit}));
  }*/

  async ngOnInit(): Promise<void> {
    //TODO in testing mode add empty components
    /*
    if(this.componentService.Components === undefined){
      this.componentService.Components = await this.componentService.CallEndpoint(ComponentEndPointType.Get) as ComponentModel[];
    }*/
    
    // Fallback test data if API fails or returns empty
    if(!this.componentService.Components || this.componentService.Components.length === 0){
      this.componentService.Components = [
        MapComponentData.Override({id: 1 ,name:"Title ett", iconUrl: "https://t4.ftcdn.net/jpg/16/18/52/61/360_F_1618526128_Kpdol855uNe6O7j4JFgMa4J9q9zBJLZb.jpg"}),
        MapComponentData.Override({id: 2 ,name:"Title två", iconUrl: ""}),
        MapComponentData.Override({id: 3 ,name:"Title tre", iconUrl: ""}),
        MapComponentData.Override({id: 4 ,name:"Title fyra", iconUrl: ""}),
        MapComponentData.Override({id: 5, name:"Title fem", iconUrl: ""})];
    }
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
    if(!this.componentService.Components) return;
    
    for (const item of this.componentService.Components) {
      const el = document.getElementById(item.id.toString());
      if(el) {
        this.render.setStyle(el, 'height', item.componentSettings?.height + 'px');
        this.render.setStyle(el, 'width', item.componentSettings?.width + 'px');
        this.render.setStyle(el, 'flex', '0 0 ' + item.componentSettings?.width + 'px');
      }
    }
  }
}
