import {ModalType} from "../Types/modaltypes.enum";
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
  type!: ModalType;
  component!: ComponentModel
  selectedDropDownValue?: number;
}
