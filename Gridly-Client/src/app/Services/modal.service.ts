import { ElementRef, Injectable, ViewChild} from "@angular/core";
import {MatDialog, MatDialogRef} from "@angular/material/dialog";
import {ModalFormType} from "../Types/modalForm.types.enum";
import {ModalComponentForm} from "../Components/Modals/ModalsComponent/Component/modal.component-form";
import {TextStringsUtil} from "../Constants/text.strings.util";
import {ModalViewModel} from "../Models/ModalView.Model";
import {ComponentService} from "./component.service";
import {IconModel} from "../Models/Icon.Model";
import {SetModalComponentFormData, SetModalPromptData} from "../Utils/viewModel.factory";
import {ModalPrompt} from "../Components/Modals/ModalsComponent/Prompts/modal.prompt";
import {ComponentType} from "@angular/cdk/portal";
import {Subject} from "rxjs";
import {EndPointType} from "../Types/endPoint.type.enum";
import {ComponentModel} from "../Models/Component.Model";

@Injectable({providedIn: 'root'})
export class ModalService{
  canSubmit:boolean = false;
  componentAsString:string = "";
  @ViewChild('fileInput') fileInputRef!: ElementRef<HTMLInputElement>;
  public resetFile$ = new Subject<void>();

  constructor(private dialog: MatDialog, protected componentService: ComponentService) {}

  GetModalType(modalType: ModalViewModel ){
      this.BuildModalTypeData(modalType);
  }

  Submit(modalType: ModalViewModel)  {
    switch (modalType.type) {
      case ModalFormType.Add:
          this.componentService.AddNewComponent(modalType);
        break;
      case ModalFormType.Edit:
          this.componentService.EditComponentData(modalType);
          break;
      case ModalFormType.Delete:
         this.componentService.DeleteComponent(modalType);
        break;
      default:
         console.error("Unknown modal type: " + modalType.type);
        break;
    }
  }

  public ResetImageData(): void {
    this.NotifyComponentToResetFileInput();
  }

  private NotifyComponentToResetFileInput(): void {
    this.resetFile$.next();
  }

  public async CanSubmit(viewModel: ModalViewModel): Promise<boolean> {
      switch (viewModel.type) {
        case ModalFormType.Add:
          this.canSubmit = this.NoEmptyInputFields(viewModel.component);
          return this.canSubmit;
        case ModalFormType.Edit:
          if(this.componentAsString === ""){
           this.componentAsString = JSON.stringify(await this.componentService.CallEndpoint(EndPointType.GetById, viewModel) as ComponentModel)
          }
          if(this.NoEmptyInputFields(viewModel.component) && this.componentAsString !== ""){
            this.canSubmit = JSON.stringify(viewModel.component) !== this.componentAsString;
          }
          return this.canSubmit;
        default:
          return this.canSubmit;
      }
  }

  private NoEmptyInputFields(component: ComponentModel): boolean{
    return this.componentService.CheckComponentData(component) &&
      (this.componentService.IconDataSet(component) || this.componentService.IconUrlSet(component));
  }

  Cancel(): void {
    if (this.componentService.InEditMode)
      this.componentService.SwitchEditMode();
    if (this.componentService.InResizeMode)
      this.componentService.SwitchResizeMode();
    if (this.componentService.InDragMode)
      this.componentService.SwitchDragMode();

    this.ResetFormData();
    window.location.reload();
  }

  OnFileUpload(event:any): IconModel | undefined{
    if (event.target.files.length > 0) {
      const file = event.target.files[0];

      if(file.type.includes('svg') ||
        file.type.includes('png') ||
        file.type.includes('jpg') ||
        file.type.includes('jpeg') ||
        file.type.includes('ico'))
      {
        let iconData = new IconModel;
        const reader = new FileReader();
        reader.onload = () => {
          iconData.base64Data = reader.result as string;
          iconData.base64Data = iconData.base64Data.split(',')[1];
        };
        reader.readAsDataURL(file);
        let nameSplit = file.name.split('.');
        iconData.name = nameSplit[0];
        iconData.type = nameSplit[1];
        return iconData;
      }
    }
    return undefined;
  }

  private BuildModalTypeData(modalType: ModalViewModel): void {
    switch (modalType.type) {
      case ModalFormType.Add:
        this.openModal(SetModalComponentFormData({
          title: TextStringsUtil.ModalAddComponentTitle,
          acceptBtnTitle: TextStringsUtil.ModalAddComponentAcceptBtnTitle,
          closeBtnTitle: TextStringsUtil.ModalAddComponentCancelBtnTitle,
          description: '',
          inputNameTitle: TextStringsUtil.ModalAddComponentInputNameTitle,
          inputUrlTitle: TextStringsUtil.ModalAddComponentInputUrlTitle,
          dropDownTitleOne: TextStringsUtil.ModalAddComponentDropDownOptionTitleOne,
          dropDownTitleTwo: TextStringsUtil.ModalAddComponentDropDownOptionTitleTwo,
          linkToImageTitle: TextStringsUtil.ModalAddComponentLinkToImageTitle,
          type: modalType.type,
          component: modalType.component}));
        break;
      case ModalFormType.Edit:
        this.openModal(SetModalComponentFormData({
          title: TextStringsUtil.ModalEditComponentTitle,
          acceptBtnTitle: TextStringsUtil.ModalEditComponentAcceptBtnTitle,
          closeBtnTitle: TextStringsUtil.ModalEditComponentCancelBtnTitle,
          description: '',
          inputNameTitle: TextStringsUtil.ModalEditComponentInputNameTitle,
          inputUrlTitle: TextStringsUtil.ModalEditComponentInputUrlTitle,
          dropDownTitleOne: TextStringsUtil.ModalEditComponentDropDownOptionTitleOne,
          dropDownTitleTwo: TextStringsUtil.ModalEditComponentDropDownOptionTitleTwo,
          linkToImageTitle: TextStringsUtil.ModalEditComponentLinkToImageTitle,
          type: modalType.type,
          component: modalType.component}));
        break;
      case ModalFormType.Delete:
        this.openModal(SetModalPromptData({
          title: TextStringsUtil.ModalDeleteComponentTitle,
          headerTitle: TextStringsUtil.ModalDeleteComponentHeaderTitle,
          acceptBtnTitle: TextStringsUtil.ModalDeleteComponentAcceptBtnTitle,
          closeBtnTitle: TextStringsUtil.ModalDeleteComponentCancelBtnTitle,
          description: TextStringsUtil.ModalDeleteComponentBtnTitle,
          type: modalType.type,
          component: modalType.component}));
        break;
    }
  }

  private openModal(data: ModalViewModel): MatDialogRef<any> {
    const component: ComponentType<any> =
      (data.type === ModalFormType.Delete || data.type === ModalFormType.None)
        ? ModalPrompt
        : ModalComponentForm;

    const ref = this.dialog.open(component, {
      width: '600px',
      enterAnimationDuration: '500ms',
      exitAnimationDuration: '500ms',
      backdropClass: 'custom-backdrop',
      panelClass: 'custom-panel',
    });
    ref.componentInstance.modalModel = data;
    return ref;
  }

  private ResetFormData(): void{
    this.componentService.ResetAllComponentData();
    this.ResetImageData();
  }

}
