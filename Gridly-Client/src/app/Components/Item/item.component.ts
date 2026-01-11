import { Component, Input, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ComponentService } from '../../Services/component.service';
import { SetModalComponentFormData } from '../../Utils/viewModel.factory';
import { TextStringsUtil } from '../../Constants/text.strings.util';
import { ComponentModel } from '../../Models/Component.Model';
import { ModalType } from '../../Types/modaltypes.enum';
import { CdkDragHandle } from '@angular/cdk/drag-drop';
import { PromptModalComponent } from '../ModalComponents/PromptModal/prompt-modal.component';
import { EditWidgetModalComponent } from '../ModalComponents/EditWidgetModal/edit-widget-modal.component';
import { ModalService } from '../../Services/modal.service';
import { ResizableDirective } from '../../Directives/resizable.directive';

@Component({
  selector: 'item-component',
  templateUrl: './item.component.html',
  styleUrl: './item.component.css',
  standalone: true,
  imports: [
    CommonModule,
    CdkDragHandle,
    PromptModalComponent,
    EditWidgetModalComponent,
    ResizableDirective,
  ],
})
export class ItemComponent {
  @Input() id!: number;

  @ViewChild(PromptModalComponent) promptModal!: PromptModalComponent;
  @ViewChild(EditWidgetModalComponent) editModal!: EditWidgetModalComponent;

  isEditModalOpen: boolean = false;
  isDeleteModalOpen: boolean = false;

  constructor(
    public componentService: ComponentService,
    private modalService: ModalService
  ) {}

  protected IconFilePath(item: ComponentModel): string {
    return 'Assets/Icons/' + item.iconData?.name + '.' + item.iconData?.type;
  }

  get CurrentComponent(): ComponentModel | undefined {
    return this.componentService.GetComponentById(this.id);
  }

  get CurrentComponentId(): number {
    const component = this.componentService.GetComponentById(this.id);
    if (component) {
      this.componentService.Component = component;
      return component.id;
    }
    return this.id;
  }

  hasBaseData(component: ComponentModel): boolean {
    return (
      component.name !== undefined &&
      component.name !== '' &&
      component.url !== undefined &&
      component.url !== '' &&
      (component.iconData !== undefined || component.iconUrl !== undefined)
    );
  }

  protected handleSelect(t: any) {
    this.modalService.submit(
      SetModalComponentFormData({ type: ModalType.Edit })
    );
  }

  handleModalChange(modalId: number): void {
    if (modalId === this.id) {
      this.isEditModalOpen = false;
      this.isDeleteModalOpen = false;
    }
  }

  protected handleSubmit(event: {component: ComponentModel; modalType: ModalType }) {
    debugger;
    switch (event.modalType) {
      case ModalType.Add:
        //this.componentService.AddNewComponent(newComponent);
        break;
      case ModalType.Edit:
        //this.componentService.EditComponentData(event.component);
        break;
      case ModalType.Delete:
        this.componentService.DeleteComponent(event.component);
        break;
    }
  }

  OpenEditModal(componentId: number): void {
    if (componentId === this.id) {
      this.isEditModalOpen = true;
    }
  }

  OpenDeleteModal(componentId: number): void {
    if (componentId === this.id) {
      this.isDeleteModalOpen = true;
    }
  }

  protected readonly TextStringsUtil = TextStringsUtil;
  protected readonly SetModalComponentFormData = SetModalComponentFormData;
}
