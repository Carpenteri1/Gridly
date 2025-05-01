import {Component, ElementRef, Injectable, OnInit, Renderer2} from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedService } from '../../Services/shared.service';
import {HandleComponent} from "../modals/handleComponent/handle.component";
import {ResizableDirective} from "../../Directives/resizable.directive";

@Component({
  selector: 'basic-component',
  imports: [CommonModule, HandleComponent, ResizableDirective],
  templateUrl: './basic.component.html',
  standalone: true,
  styleUrls: ['./basic.component.css']
})

@Injectable({ providedIn: 'root' })
export class BasicComponent implements OnInit {
  modalTitle = "Edit";
  modalButtonTheme ="btn btn-modal";
  modalButtonIcon = "bi bi-three-dots";
  modelBindId = "editComponentModalLabel";
  modalDropDownId = "editComponentModal";
  public resizableActive = false;
  public resizingItem!: any;

  constructor(
    public sharedService: SharedService,
    public handleComponent: HandleComponent,
    private render: Renderer2,
    private el: ElementRef) {}

  ngOnInit() {
    this.sharedService.LoadComponentList();
    //this.SetComponentLayout();  TODO fix
    this.resizableActive = false;
  }

   ActivateResizeStatus(item: any): void {
    this.resizableActive = true;
    this.resizingItem = item;
   }

    DisableResizeStatus(): void {
     this.resizableActive = false;
     this.sharedService.EditComponent(this.resizingItem);
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

  /*private SetComponentLayout(){
     this.render.setStyle(this.el.nativeElement, 'height', this.resizingItem.componentSettings.height + 'px');
     this.resizingItem.setStyle(this.el.nativeElement, 'flex', '0 0 '+ this.resizingItem.componentSettings.width  + 'px');
  }*/
}
