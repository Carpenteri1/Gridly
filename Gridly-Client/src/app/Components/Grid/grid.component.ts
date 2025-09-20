import {AfterViewChecked, Component, ElementRef, OnInit, Renderer2} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ComponentService} from "../../Services/component.service";
import {ModalViewModel} from "../../Models/ModalView.Model";
import {ComponentEndPointType} from "../../Types/endPoint.type.enum";
import {StickyFooterComponent} from "../StickyFooter/stickyFooter.component";
import {ItemComponent} from "../Item/item.component";
import {CdkDrag, CdkDragDrop, CdkDropList, moveItemInArray} from "@angular/cdk/drag-drop";
import {ResizableDirective} from "../../Directives/resizable.directive";
import {ComponentModel} from "../../Models/Component.Model";
import {MapComponentData} from "../../Utils/componentModal.factory";

@Component({
  selector: 'grid-component',
  imports: [CommonModule, StickyFooterComponent, ItemComponent, CdkDropList, CdkDrag, ResizableDirective],
  templateUrl: './grid.component.html',
  standalone: true,
  styleUrls: ['./grid.component.css']
})

export class GridComponent implements AfterViewChecked, OnInit{
  protected modalModel!: ModalViewModel;

  constructor(
    protected componentService: ComponentService,
    private render: Renderer2,
    private el: ElementRef) {
  }

  async ngOnInit(): Promise<void> {
  if( this.componentService.Components === undefined ){
    this.componentService.Components = [
      MapComponentData.Override({id:1,componentSettings:{width:250,height:250}},new ComponentModel()),
      MapComponentData.Override({id:2,componentSettings:{width:250,height:250}},new ComponentModel()),
      MapComponentData.Override({id:3,componentSettings:{width:250,height:250}},new ComponentModel()),
      MapComponentData.Override({id:4,componentSettings:{width:250,height:250}},new ComponentModel()),
      MapComponentData.Override({id:5,componentSettings:{width:250,height:250}},new ComponentModel()),
      MapComponentData.Override({id:5,componentSettings:{width:250,height:250}},new ComponentModel()),
      MapComponentData.Override({id:5,componentSettings:{width:250,height:250}},new ComponentModel()),
    ];
  }

   /* if(this.componentService.Components === undefined){
      this.componentService.Components = await this.componentService.CallEndpoint(ComponentEndPointType.Get) as ComponentModel[];
    }*/
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
