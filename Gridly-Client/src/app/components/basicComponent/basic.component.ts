import { AfterViewChecked, Component, ElementRef, Injectable, OnInit, Renderer2 } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedService } from '../../Services/shared.service';
import { HandleComponent } from "../modals/handleComponent/handle.component";
import { ResizableDirective } from "../../Directives/resizable.directive";
import { CdkDrag, CdkDragDrop, CdkDropList, moveItemInArray} from '@angular/cdk/drag-drop';

@Component({
  selector: 'basic-component',
  imports: [CdkDrag, CdkDropList, CommonModule, HandleComponent, ResizableDirective],
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
  public canMove = false;
  public itemComponent!: any;

  constructor(
    public sharedService: SharedService,
    public handleComponent: HandleComponent,
    private render: Renderer2,
    private el: ElementRef) {}

  ngOnInit() {
    this.sharedService.LoadComponentList();
    this.resizableActive = false;
    this.canMove = true;
  }

  ngAfterViewChecked() {
    this.SetComponentLayout();
  }

   ActivateResize(item: any): void {
   // this.resizableActive = true;
    this.itemComponent = item;
   }

    DisableResize(): void {
     //this.resizableActive = false;
     //this.sharedService.EditComponent(this.resizingItem); TODO add later
    }

  ActivateMove(item: any): void {
    //this.canMove = !this.canMove;
    if(this.canMove) this.itemComponent = item;
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
    if (!this.canMove) return;
    moveItemInArray(this.sharedService.flexItems, event.previousIndex, event.currentIndex);
  }

}
