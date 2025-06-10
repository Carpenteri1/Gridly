import {Injectable} from "@angular/core";
import {MatDialog, MatDialogRef} from "@angular/material/dialog";
import {ModalFormType} from "../Types/modalForm.types.enum";
import {ModalsComponent} from "../Components/Modals/ModalsComponent/modals.component";
import {TextStringsUtil} from "../Constants/text.strings.util";
import {ModalViewModel} from "../Models/ModalView.Model";
import {ComponentService} from "./component.service";
import {IconModel} from "../Models/Icon.Model";
import {SetComponentModalData} from "../Utils/viewModel.factory";
import {SetComponentData, SetIconData} from "../Utils/componentModal.factory";

@Injectable({providedIn: 'root'})
export class ModalService{

  constructor(private dialog: MatDialog, protected componentService: ComponentService) {}

  GetModalType(type: ModalFormType ){
      let viewData = this.SetModalType(type);
      this.BuildModalType(viewData);
  }

  private BuildModalType(modalType: ModalViewModel): void {
    switch (modalType.type) {
      case ModalFormType.Add:
        this.openModal(SetComponentModalData({
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
        this.openModal(SetComponentModalData({
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
    }
  }

  Submit(modalType: ModalViewModel) {
    switch (modalType.type) {
      case ModalFormType.Add:
        return this.componentService.AddComponent(modalType.component ?? SetComponentData());
      case ModalFormType.Edit:
        return this.componentService.EditComponent(modalType.component ?? SetComponentData());
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
      this.componentService.iconData = SetIconData();
      const file = event.target.files[0];
      this.SetFileNameAndType(file.name);

      if(this.componentService.iconData.fileType === 'svg' ||
        this.componentService.iconData.fileType === 'png' ||
        this.componentService.iconData.fileType === 'jpg' ||
        this.componentService.iconData.fileType === 'jpeg' ||
        this.componentService.iconData.fileType === 'ico')
      {
        const reader = new FileReader();
        reader.onload = () => {
          this.componentService.iconData.base64Data = reader.result as string;
          this.componentService.iconData.base64Data =  this.componentService.iconData.base64Data.split(',')[1];
        };
        reader.readAsDataURL(file);
        return this.componentService.iconData;
      }
    }
    return undefined;
  }

  private SetModalType(type: ModalFormType) : ModalViewModel {
    return SetComponentModalData({type:type});
  }

  private openModal(data: ModalViewModel): MatDialogRef<ModalsComponent> {
    const ref =
      this.dialog.open(ModalsComponent, {
      width: '600px',
      enterAnimationDuration: '500ms',
      exitAnimationDuration: '500ms',
    });
    ref.componentInstance.modalModel = data;
    return ref;
  }

  private SetFileNameAndType(inputName: string){
   let result = inputName.split('.');
   this.componentService.iconData.name = result[0];
   this.componentService.iconData.fileType = result[1];
  }
}
