import {AfterViewChecked, Component, ElementRef, Injectable, OnInit, Renderer2} from '@angular/core';
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
export class BasicComponent implements OnInit, AfterViewChecked {
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
    this.resizableActive = false;
  }

  ngAfterViewChecked() {
    this.SetComponentLayout();
  }

   ActivateResize(item: any): void {
    this.resizableActive = true;
    this.resizingItem = item;
   }

    DisableResize(): void {
     this.resizableActive = false;
     //this.sharedService.EditComponent(this.resizingItem); TODO add later
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
}
