import {AfterViewChecked, Component, ElementRef, Renderer2} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ComponentService} from "../../Services/component.service";
import {ModalViewModel} from "../../Models/ModalView.Model";
import {EndPointType} from "../../Types/endPoint.type.enum";
import {StickyFooterComponent} from "../StickyFooter/stickyFooter.component";
import {ItemComponent} from "../Item/item.component";
import {CdkDrag, CdkDragDrop, CdkDropList, moveItemInArray} from "@angular/cdk/drag-drop";
import {ResizableDirective} from "../../Directives/resizable.directive";

@Component({
  selector: 'grid-component',
  imports: [CommonModule, StickyFooterComponent, ItemComponent, CdkDropList, CdkDrag, ResizableDirective],
  templateUrl: './grid.component.html',
  standalone: true,
  styleUrls: ['./grid.component.css']
})

export class GridComponent implements AfterViewChecked {
  protected modalModel!: ModalViewModel;

  constructor(
    protected componentService: ComponentService,
    private render: Renderer2,
    private el: ElementRef) {
    this.componentService.CallEndpoint(EndPointType.Get);

  }
  ngAfterViewChecked() {
    this.SetLayout();
  }

  Drop(event: CdkDragDrop<any[]>): void {
    if (!this.componentService.InDragMode) return;
    moveItemInArray(this.componentService.GetAllComponents, event.previousIndex, event.currentIndex);
  }

  SetLayout() {
    for (let index in this.componentService.GetAllComponents) {
      let item = this.componentService.GetAllComponents[index];
      let el = document.getElementById(item.id.toString()) == null ? this.el.nativeElement : document.getElementById(item.id.toString());
      this.render.setStyle(el, 'height', item.componentSettings?.height + 'px');
      this.render.setStyle(el, 'flex', '0 0 ' + item.componentSettings?.width + 'px');
    }
  }
}
