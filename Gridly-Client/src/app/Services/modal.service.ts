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
import {RegexStringsUtil} from "../Constants/regex.strings.util";

@Injectable({providedIn: 'root'})
export class ModalService{
  isClicked: boolean = false;

  constructor(private dialog: MatDialog, protected componentService: ComponentService) {}

  GetModalType(modalType: ModalViewModel ){
      this.BuildModalTypeData(modalType);
  }

  Submit(modalType: ModalViewModel) {
    switch (modalType.type) {
      case ModalFormType.Add:
        debugger;
        this.isClicked = this.componentService.AddComponent(modalType.component ?? SetComponentData());
        break;
      case ModalFormType.Edit:
        this.isClicked = this.componentService.EditComponent(modalType.component ?? SetComponentData());
        break;
      case ModalFormType.Delete:
        this.isClicked = this.componentService.DeleteComponent(modalType.component.id ?? SetComponentData());
        break;
      default:
        break;
    }
    this.ResetFormData()
    window.location.reload();
    return this.isClicked;
  }

  public CanEditComponent() {
    return
    this.NoEmptyInputFields &&
    JSON.stringify(this.componentService.componentEndpointService.GetComponentById) !==
    JSON.stringify(this.componentService.component);
  }

  get NoEmptyInputFields(): boolean{
    if(this.componentService.component.name !== "" && this.componentService.component.url !== ""
      //&&
      // RegexStringsUtil.urlPattern.test(this.component.url) && RegexStringsUtil.namePattern.test(this.component.name)
    ){

      if(this.componentService.component.iconData?.name !== "") (
        this.componentService.component.imageUrl !== "" &&
        this.componentService.component.imageUrl !== undefined &&
        RegexStringsUtil.imageUrlPattern.test(this.componentService.component.imageUrl))
      {
        return true;
      }
    }
    return false;
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
          iconData.base64Data = iconData.base64Data .split(',')[1];
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
          linkToImageTitle: TextStringsUtil.ModalAddComponenLinkToImageTitle,
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

  ResetFormData(): void{
    this.componentService.component.id = 0;
    this.componentService.component.name = "";
    this.componentService.component.url = "";
    this.componentService.component.titleHidden = false;
    this.componentService.component.imageHidden = false;
    this.ResetIconData();
  }

  ResetIconData(){
    this.componentService.component.iconData = undefined;
    this.componentService.component.imageUrl = "";
  }
}
