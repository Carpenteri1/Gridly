import {ModalFormType} from "../Types/modalForm.types.enum";
import {IComponentModel} from "./IComponent.Model";
export interface IModalsModel {
  title: string;
  acceptBtnTitle: string;
  closeBtnTitle: string;
  description: string;
  inputNameTitle: string;
  inputUrlTitle: string;
  dropDownTitleOne: string;
  dropDownTitleTwo: string;
  linkToImageTitle: string;
  type: ModalFormType;
  component: IComponentModel
  selectedDropDownValue: number;
}
