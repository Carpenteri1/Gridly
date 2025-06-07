import {IconModel} from "./Icon.Model";
import {ComponentSettingsModel} from "./ComponentSettings.Model";

export class ComponentModel {
  id: number;
  name?: string;
  url?: string;
  iconData?: IconModel;
  imageUrl?: string;
  imageHidden?: boolean;
  titleHidden?: boolean;
  componentSettings? : ComponentSettingsModel;

  constructor(
    id: number,
    name?: string,
    url?: string,
    imageUrl?: string | undefined,
    imageHidden?: boolean,
    titleHidden?: boolean,
    iconData?: IconModel,
    componentSettings?: ComponentSettingsModel) {
    this.id = id;
    this.name = name;
    this.url = url;
    this.imageUrl = imageUrl;
    this.imageHidden = imageHidden;
    this.titleHidden = titleHidden;
    this.iconData = iconData;
    this.componentSettings = componentSettings;
  }
}
