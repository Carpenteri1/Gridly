import {AfterViewChecked, Component, ElementRef, OnInit, Renderer2} from '@angular/core';
import { CommonModule} from '@angular/common';
import { ComponentService } from "../../Services/component.service";
import { ModalViewModel } from "../../Models/ModalView.Model";
import { ItemComponent } from "../Item/item.component";
import { CdkDrag, CdkDragDrop, CdkDropList, moveItemInArray } from "@angular/cdk/drag-drop";
import { ComponentEndPointType } from "../../Types/endPoint.type.enum";
import { ComponentModel } from "../../Models/Component.Model";

@Component({
  selector: 'grid-component',
  imports: [CommonModule, CdkDropList, CdkDrag, ItemComponent],
  templateUrl: './grid.component.html',
  standalone: true,
  styleUrls: ['./grid.component.css'],
})

export class GridComponent implements AfterViewChecked, OnInit{
  protected modalModel!: ModalViewModel;

  constructor(
    protected componentService: ComponentService,
    private render: Renderer2) {
  }
  
  async ngOnInit(): Promise<void> {

    if(this.componentService.Components === undefined){
      this.componentService.Components = await this.componentService.CallEndpoint(ComponentEndPointType.Get) as ComponentModel[];
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
