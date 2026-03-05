import {ModalViewModel} from "../Models/ModalView.Model";
import {ModalType} from "../Types/modaltypes.enum";
import {MapComponentData} from "./componentModel.factory";

export function SetModalComponentFormData(override: Partial<ModalViewModel> = {}) : ModalViewModel{
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
    type: override.type ?? ModalType.None,
    component: override.component ?? MapComponentData(),
    selectedDropDownValue: override.selectedDropDownValue ?? 0
  } as ModalViewModel;
}

export function SetModalPromptData(override: Partial<ModalViewModel> = {}) : ModalViewModel{
  return {
    title: override.title ?? "",
    acceptBtnTitle: override.acceptBtnTitle ?? "",
    closeBtnTitle: override.closeBtnTitle ?? "",
    description: override.description ?? "",
    type: override.type ?? ModalType.None,
    component: override.component ?? MapComponentData(),
  } as ModalViewModel;
}
