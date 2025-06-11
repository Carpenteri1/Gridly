import {AfterViewChecked, Component, ElementRef, OnInit, Renderer2} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ResizableDirective} from "../../Directives/resizable.directive";
import {CdkDrag, CdkDragDrop, CdkDropList, moveItemInArray} from '@angular/cdk/drag-drop';
import {ComponentModel} from "../../Models/Component.Model";
import {ModalFormType} from "../../Types/modalForm.types.enum";
import {ComponentService} from "../../Services/component.service";
import {ModalService} from "../../Services/modal.service";
import {ModalViewModel} from "../../Models/ModalView.Model";
import {SetModalComponentFormData} from "../../Utils/viewModel.factory";

@Component({
  selector: 'basic-component',
  imports: [CdkDrag, CdkDropList, CommonModule, ResizableDirective],
  templateUrl: './basic.component.html',
  standalone: true,
  styleUrls: ['./basic.component.css']
})

export class BasicComponent implements OnInit, AfterViewChecked {
  protected resizableActive!: boolean;
  protected component!: ComponentModel;
  protected components!: ComponentModel[];
  protected modalModel!: ModalViewModel;

  constructor(
    protected componentService: ComponentService,
    protected modalService: ModalService,
    private render: Renderer2,
    private el: ElementRef) {}

  async ngOnInit() {
    this.components = await this.componentService.GetComponents();
  }

  ngAfterViewChecked() {
    this.SetComponentLayout();
  }

  SetComponentLayout() {
    for (let index in this.components) {
      let item = this.components[index];
      let el = document.getElementById(item.id.toString()) == null ? this.el.nativeElement : document.getElementById(item.id.toString());
      this.render.setStyle(el, 'height', item.componentSettings?.height + 'px');
      this.render.setStyle(el, 'flex', '0 0 ' + item.componentSettings?.width + 'px');
    }
  }

   ActivateResize(item: any): void {
    this.resizableActive = true;
    this.component = item;
   }

    DisableResize(): void {
     this.resizableActive = false;
    //this.componentService.EditComponent();
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

  Drop(event: CdkDragDrop<any[]>): void {
    if (!this.component.dragMode) return;
    moveItemInArray(this.components, event.previousIndex, event.currentIndex);
  }

  protected readonly FormType = ModalFormType;
  protected readonly SetModalComponentFormData = SetModalComponentFormData;
}
