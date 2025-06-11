import {Injectable} from "@angular/core";
import {MatDialog, MatDialogRef} from "@angular/material/dialog";
import {ModalFormType} from "../Types/modalForm.types.enum";
import {ModalComponentForm} from "../Components/Modals/ModalsComponent/Component/modal.component-form";
import {TextStringsUtil} from "../Constants/text.strings.util";
import {ModalViewModel} from "../Models/ModalView.Model";
import {ComponentService} from "./component.service";
import {IconModel} from "../Models/Icon.Model";
import {SetModalComponentFormData, SetModalPromptData} from "../Utils/viewModel.factory";
import {SetComponentData, SetIconData} from "../Utils/componentModal.factory";
import {ModalPrompt} from "../Components/Modals/ModalsComponent/Prompts/modal.prompt";
import {ComponentType} from "@angular/cdk/portal";

@Injectable({providedIn: 'root'})
export class ModalService{

  constructor(private dialog: MatDialog, protected componentService: ComponentService) {}

  GetModalType(modalType: ModalViewModel ){
      this.BuildModalTypeData(modalType);
  }

  Submit(modalType: ModalViewModel) {
    switch (modalType.type) {
      case ModalFormType.Add:
        return this.componentService.AddComponent(modalType.component ?? SetComponentData());
      case ModalFormType.Edit:
        return this.componentService.EditComponent(modalType.component ?? SetComponentData());
      case ModalFormType.Delete:
        return this.componentService.DeleteComponent(modalType.component.id ?? SetComponentData());
      default:
        return false;
    }
  }

  Cancel(): void {
    if (this.componentService.editMode)
      this.componentService.SwitchEditMode();
    if (this.componentService.resizeMode)
      this.componentService.SwitchResizeMode();
    if (this.componentService.dragMode)
      this.componentService.SwitchDragMode();
  }

  OnFileUpload(event:any): IconModel | undefined{
    if (event.target.files && event.target.files.length > 0) {
      this.componentService.component.iconData = SetIconData();
      const file = event.target.files[0];
      this.SetFileNameAndType(file.name);

      if(this.componentService.component.iconData.fileType === 'svg' ||
        this.componentService.component.iconData.fileType === 'png' ||
        this.componentService.component.iconData.fileType === 'jpg' ||
        this.componentService.component.iconData.fileType === 'jpeg' ||
        this.componentService.component.iconData.fileType === 'ico')
      {
        const reader = new FileReader();
        reader.onload = () => {
          this.componentService.component.iconData!.base64Data = reader.result as string;
          this.componentService.component.iconData!.base64Data =  this.componentService.component.iconData!.base64Data.split(',')[1];
        };
        reader.readAsDataURL(file);
        return this.componentService.component.iconData;
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
          closeBtnTitle: TextStringsUtil.ModalAddComponentCloseBtnTitle,
          description: '',
          inputNameTitle: TextStringsUtil.ModalAddComponentInputNameTitle,
          inputUrlTitle: TextStringsUtil.ModalAddComponentInputUrlTitle,
          dropDownTitleOne: TextStringsUtil.ModalAddComponentDropDownOptionTitleOne,
          dropDownTitleTwo: TextStringsUtil.ModalAddComponentDropDownOptionTitleTwo,
          linkToImageTitle: TextStringsUtil.ModalAddComponenLinkToImageTitle,
          type: modalType.type,
          component: modalType.component}));
        break;
      case ModalFormType.Edit:
        this.openModal(SetModalComponentFormData({
          title: TextStringsUtil.ModalEditComponentTitle,
          acceptBtnTitle: TextStringsUtil.ModalEditComponentAcceptBtnTitle,
          closeBtnTitle: TextStringsUtil.ModalEditComponentCloseBtnTitle,
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
          closeBtnTitle: TextStringsUtil.ModalDeleteComponentCloseBtnTitle,
          description: TextStringsUtil.ModalDeleteComponentBtnTitle,
          type: modalType.type,
          component: modalType.component}));
        break;
    }
  }

  private SetModalType(type: ModalFormType) : ModalViewModel {
    return SetModalComponentFormData({type:type});
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
    });
    ref.componentInstance.modalModel = data;
    return ref;
  }

  private SetFileNameAndType(inputName: string){
   let result = inputName.split('.');
   this.componentService.component.iconData!.name = result[0];
   this.componentService.component.iconData!.fileType = result[1];
  }
}
