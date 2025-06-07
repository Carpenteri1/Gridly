import {IconModel} from "./Icon.Model";
import {ComponentSettingsModel} from "./ComponentSettings.Model";

export interface IComponentModel {
  Id: number;
  Name?: string;
  Url?: string;
  IconData?: IconModel;
  ImageUrl?: string;
  ImageHidden?: boolean;
  TitleHidden?: boolean;
  ComponentSettings? : ComponentSettingsModel;
}
