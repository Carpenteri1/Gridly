import {IComponentSettingsModel} from "./IComponentSettings.Model";
import {IIconModel} from "./IIcon.Model";

export interface IComponentModel {
  id: number;
  name?: string;
  url?: string;
  iconData?: IIconModel;
  imageUrl?: string;
  imageHidden?: boolean;
  titleHidden?: boolean;
  editMode?: boolean;
  resizeMode?: boolean;
  dragMode?: boolean;
  componentSettings? : IComponentSettingsModel;
}
