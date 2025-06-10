import {ModalViewModel} from "../Models/ModalView.Model";
import {ModalFormType} from "../Types/modalForm.types.enum";
import {SetComponentData} from "./componentModal.factory";

export function SetComponentModalData(override: Partial<ModalViewModel> = {}) : ModalViewModel{
  return {
    title: override.title ?? "",
    acceptBtnTitle: override.acceptBtnTitle ?? "",
    closeBtnTitle: override.closeBtnTitle ?? "",
    description: override.description ?? "",
    inputNameTitle: override.inputNameTitle ?? "",
    inputUrlTitle: override.inputUrlTitle ?? "",
    dropDownTitleOne: override.dropDownTitleOne ?? "",
    dropDownTitleTwo: override.dropDownTitleTwo ?? "",
    linkToImageTitle: override.linkToImageTitle ?? "",
    type: override.type ?? ModalFormType.None,
    component: override.component ?? SetComponentData(),
    selectedDropDownValue: override.selectedDropDownValue ?? 0
  } as ModalViewModel;
}
