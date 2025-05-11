import { AfterViewChecked, Component, ElementRef, Injectable, OnInit, Renderer2 } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedService } from '../../Services/shared.service';
import { ResizableDirective } from "../../Directives/resizable.directive";
import { CdkDrag, CdkDragDrop, CdkDropList, moveItemInArray } from '@angular/cdk/drag-drop';
import { HandleComponent } from "../modals/handleComponent/handle.component";
import {StringUtil} from "../../Utils/string.util";

@Component({
  selector: 'basic-component',
  imports: [CdkDrag, CdkDropList, CommonModule, ResizableDirective, HandleComponent],
  templateUrl: './basic.component.html',
  standalone: true,
  styleUrls: ['./basic.component.css']
})

export class BasicComponent implements OnInit, AfterViewChecked {
  resizableActive!: boolean;
  canResize!: boolean;
  canDrag!: boolean;
  itemComponent!: any;

  constructor(
    public sharedService: SharedService,
    private render: Renderer2,
    private el: ElementRef) {}

  ngOnInit() {
    this.sharedService.LoadComponentList();
    this.resizableActive = false;// disabled for now

    this.canDrag = false; // enabled for now
    this.canResize = false; // enabled for now
  }

  ngAfterViewChecked() {
    this.SetComponentLayout();
  }

   ActivateResize(item: any): void {
    this.resizableActive = true;
    this.itemComponent = item;
   }

    DisableResize(): void {
     //this.resizableActive = false;
     //this.sharedService.EditComponent(this.resizingItem); TODO add later
    }

    EditComponent() {
      this.sharedService.EditComponent(this.itemComponent,this.itemComponent.iconData);
    }

  HaveIconSet(name:string | undefined):boolean{
    return name !== undefined && name != null && name !== "";
  }
  HaveImagUrlSet(imageUrl:string | undefined):boolean{
    return imageUrl !== undefined && imageUrl != null && imageUrl !== "";
  }

  IconFilePath(name:string | undefined, fileType:string | undefined): string {
    return "Assets/Icons/" + name + "." + fileType;
  }

  Remove(id: number): void {
    this.sharedService.RemoveComponent(id);
  }

  SetComponentLayout() {
    for (let index in this.sharedService.flexItems) {
      let item = this.sharedService.flexItems[index];
      let el = document.getElementById(item.id.toString()) == null ? this.el.nativeElement : document.getElementById(item.id.toString());
      this.render.setStyle(el, 'height', item.componentSettings?.height + 'px');
      this.render.setStyle(el, 'flex', '0 0 ' + item.componentSettings?.width + 'px');
    }
  }

  Drop(event: CdkDragDrop<any[]>): void {
    if (!this.canDrag) return;
    moveItemInArray(this.sharedService.flexItems, event.previousIndex, event.currentIndex);
  }

  protected readonly StringUtil = StringUtil;
}
