import {FormType} from "../Types/form.types.enum";
import {IComponentModel} from "./IComponent.Model";
export interface IModalsModel {
  Title: string;
  AcceptBtnTitle: string;
  CloseBtnTitle: string;
  Description: string;
  Type: FormType;
  Component: IComponentModel
}
