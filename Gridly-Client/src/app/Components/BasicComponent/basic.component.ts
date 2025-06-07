import { AfterViewChecked, Component, ElementRef, OnInit, Renderer2 } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ComponentEndpointService } from '../../Services/endpoints/component.endpoint.service';
import { ResizableDirective } from "../../Directives/resizable.directive";
import { CdkDrag, CdkDragDrop, CdkDropList, moveItemInArray } from '@angular/cdk/drag-drop';
import { TextStringsUtil } from "../../Constants/text.strings.util";
import { ComponentModel } from "../../Models/Component.Model";
import { Observable } from "rxjs";
import {FormType} from "../../Types/form.types.enum";

@Component({
  selector: 'basic-component',
  imports: [CdkDrag, CdkDropList, CommonModule, ResizableDirective],
  templateUrl: './basic.component.html',
  standalone: true,
  styleUrls: ['./basic.component.css']
})

export class BasicComponent implements OnInit, AfterViewChecked {
  type!: FormType;
  resizableActive!: boolean;
  canResize!: boolean;
  canDrag!: boolean;
  itemComponent!: ComponentModel;
  itemComponents$!: Observable<ComponentModel[]>;
  itemComponents!: ComponentModel[];

  constructor(
    public endpointService: ComponentEndpointService,
    private render: Renderer2,
    private el: ElementRef) {}

  ngOnInit() {
    this.itemComponents$ = this.endpointService.GetComponents();
    this.itemComponents$.subscribe(data => this.itemComponents = data);

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
      this.endpointService.EditComponent(this.itemComponent,this.itemComponent.iconData);
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
    this.endpointService.RemoveComponent(id);
  }

  SetComponentLayout() {
    for (let index in this.itemComponents) {
      let item = this.itemComponents[index];
      let el = document.getElementById(item.id.toString()) == null ? this.el.nativeElement : document.getElementById(item.id.toString());
      this.render.setStyle(el, 'height', item.componentSettings?.height + 'px');
      this.render.setStyle(el, 'flex', '0 0 ' + item.componentSettings?.width + 'px');
    }
  }

  Drop(event: CdkDragDrop<any[]>): void {
    if (!this.canDrag) return;
    moveItemInArray(this.itemComponents, event.previousIndex, event.currentIndex);
  }

  protected readonly StringUtil = TextStringsUtil;
}
