import {Injectable} from "@angular/core";
import {MatDialog, MatDialogRef} from "@angular/material/dialog";
import {ModalFormType} from "../Types/modalForm.types.enum";
import {ModalsComponent} from "../Components/Modals/ModalsComponent/modals.component";
import {TextStringsUtil} from "../Constants/text.strings.util";
import {IComponentModel} from "../Models/IComponent.Model";
import {CreateComponentData} from "../Utils/componentModal.factory";
import {IModalsModel} from "../Models/IModals.Model";
import {CreateComponentModalData} from "../Utils/viewModel.factory";
import {ComponentService} from "./component.service";

@Injectable({providedIn: 'root'})
export class ModalService{

  constructor(private dialog: MatDialog, public componentService: ComponentService) {}

  GetModalType(type:ModalFormType, component?: IComponentModel): void {
    switch (type) {
      case ModalFormType.Add:
        this.openModal(CreateComponentModalData(
          TextStringsUtil.ModalAddComponentTitle,
          TextStringsUtil.ModalAddComponentAcceptBtnTitle,
          TextStringsUtil.ModalAddComponentCloseBtnTitle,
          '',
          TextStringsUtil.ModalAddComponentInputNameTitle,
          TextStringsUtil.ModalAddComponentInputUrlTitle,
          TextStringsUtil.ModalAddComponentDropDownOptionTitleOne,
          TextStringsUtil.ModalAddComponentDropDownOptionTitleTwo,
          TextStringsUtil.ModalAddComponenLinkToImageTitle,
          type,
          CreateComponentData()));
        break;
      case ModalFormType.Edit:
        this.openModal(CreateComponentModalData(
          TextStringsUtil.ModalEditComponentTitle,
          TextStringsUtil.ModalEditComponentAcceptBtnTitle,
          TextStringsUtil.ModalEditComponentCloseBtnTitle,
          '',
          TextStringsUtil.ModalEditComponentInputNameTitle,
          TextStringsUtil.ModalEditComponentInputUrlTitle,
          TextStringsUtil.ModalEditComponentDropDownOptionTitleOne,
          TextStringsUtil.ModalEditComponentDropDownOptionTitleTwo,
          TextStringsUtil.ModalEditComponentLinkToImageTitle,
          type,
          CreateComponentData(component)));
        break;
    }
  }

  Submit(type: ModalFormType) {
    switch (type) {
      case ModalFormType.Add:
        return this.componentService.AddComponent();
      case ModalFormType.Edit:
        return this.componentService.EditComponent();
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

  OnFileUpload(event:any):void{
    /*if (event.target.files && event.target.files.length > 0) {
      const file = event.target.files[0];
      this.SetFileNameAndType(file.name);

      if(this.iconData.fileType === 'svg' ||
        this.iconData.fileType === 'png' ||
        this.iconData.fileType === 'jpg' ||
        this.iconData.fileType === 'jpeg' ||
        this.iconData.fileType === 'ico')
      {
        const reader = new FileReader();
        reader.onload = () => {
          this.iconData.base64Data = reader.result as string;
          this.iconData.base64Data =  this.iconData.base64Data.split(',')[1];
        };
        reader.readAsDataURL(file);
      }
    }*/
  }



  private openModal(data: IModalsModel): MatDialogRef<ModalsComponent> {
    const ref = this.dialog.open(ModalsComponent, {
      width: '600px',
      enterAnimationDuration: '500ms',
      exitAnimationDuration: '500ms',
    });
    ref.componentInstance.viewModel = data;
    return ref;
  }

  private SetFileNameAndType(inputName: string){
   /* let result = inputName.split('.');
    this.iconData.name = result[0];
    this.iconData.fileType = result[1];*/
  }
}
