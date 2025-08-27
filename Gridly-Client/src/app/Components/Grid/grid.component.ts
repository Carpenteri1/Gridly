import {AfterViewChecked, Component, ElementRef, OnInit, Renderer2} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ComponentService} from "../../Services/component.service";
import {ModalViewModel} from "../../Models/ModalView.Model";
import {EndPointType} from "../../Types/endPoint.type.enum";
import {StickyFooterComponent} from "../StickyFooter/stickyFooter.component";
import {ItemComponent} from "../Item/item.component";
import {CdkDrag, CdkDragDrop, CdkDropList, moveItemInArray} from "@angular/cdk/drag-drop";
import {ResizableDirective} from "../../Directives/resizable.directive";
import {ComponentModel} from "../../Models/Component.Model";

@Component({
  selector: 'grid-component',
  imports: [CommonModule, StickyFooterComponent, ItemComponent, CdkDropList, CdkDrag, ResizableDirective],
  templateUrl: './grid.component.html',
  standalone: true,
  styleUrls: ['./grid.component.css']
})

export class GridComponent implements AfterViewChecked, OnInit{
  protected modalModel!: ModalViewModel;
  public components!: ComponentModel[];

  constructor(
    protected componentService: ComponentService,
    private render: Renderer2,
    private el: ElementRef) {
  }

  async ngOnInit(): Promise<void> {
    if(this.components === undefined){
      this.components = await this.componentService.CallEndpoint(EndPointType.Get) as ComponentModel[];
    }
  }

  ngAfterViewChecked() {
    if(!this.componentService.InAnyMode &&
      this.componentService.GetAllComponents !== undefined)
    {
      this.SetLayout();
    }
  }

  protected Drop(event: CdkDragDrop<any[]>): void {
    if (!this.componentService.InDragMode) return;
    moveItemInArray(this.components, event.previousIndex, event.currentIndex);
  }

  protected SetLayout() {
    for (const item of this.componentService.GetAllComponents) {
      let el = document.getElementById(item.id.toString()) == null ? this.el.nativeElement : document.getElementById(item.id.toString());
      this.render.setStyle(el, 'height', item.componentSettings?.height + 'px');
      this.render.setStyle(el, 'flex', '0 0 ' + item.componentSettings?.width + 'px');
    }
  }
}
