import {IModalsModel} from "../Models/IModals.Model";
import {ModalFormType} from "../Types/modalForm.types.enum";
import {IComponentModel} from "../Models/IComponent.Model";
import {IIconModel} from "../Models/IIcon.Model";
export function CreateComponentModalData(
  overrideTitle: string,
  overrideAcceptBtnTitle: string,
  overrideCloseBtnTitle: string,
  overrideDescripton: string,
  overrideInputTitle: string,
  overrideInputUrl: string,
  overrideDropDownTitleOne: string,
  overrideDropDownTitleTwo: string,
  overrideLinkToImageTitle: string,
  overrideType: ModalFormType,
  overrideComponent: IComponentModel,
  overrideSelectedDropDownValue?: number) : IModalsModel{
  return {
    title: overrideTitle || "",
    acceptBtnTitle: overrideAcceptBtnTitle || "",
    closeBtnTitle: overrideCloseBtnTitle || "",
    description: overrideDescripton || "",
    inputNameTitle: overrideInputTitle || "",
    inputUrlTitle: overrideInputUrl || "",
    dropDownTitleOne: overrideDropDownTitleOne || "",
    dropDownTitleTwo: overrideDropDownTitleTwo || "",
    linkToImageTitle: overrideLinkToImageTitle || "",
    type: overrideType || ModalFormType.None,
    component: overrideComponent || {
      id: 0,
      name: "",
      url: "",
      imageUrl: "",
      imageHidden: false,
      titleHidden: false,
      iconData: {fileType: "", name: "", base64Data: ""} as IIconModel,
      componentSettings: {width: 0, height: 0}
    },
    selectedDropDownValue: overrideSelectedDropDownValue || 0
  } as IModalsModel;
}
