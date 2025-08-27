import {ModalFormType} from "../Types/modalForm.types.enum";
import {ComponentModel} from "./Component.Model";
export class ModalViewModel {
  title!: string;
  headerTitle?: string;
  acceptBtnTitle!: string;
  closeBtnTitle!: string;
  description!: string;
  inputNameTitle?: string;
  inputUrlTitle?: string;
  dropDownTitleOne?: string;
  dropDownTitleTwo?: string;
  linkToImageTitle?: string;
  type!: ModalFormType;
  component!: ComponentModel
  selectedDropDownValue?: number;
}
