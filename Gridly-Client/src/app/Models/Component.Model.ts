import {ComponentSettingsModel} from "./ComponentSettings.Model";
import {IconModel} from "./Icon.Model";

export class ComponentModel {
  id!: number;
  name!: string;
  url!: string;
  iconData?: IconModel | undefined;
  iconUrl?: string;
  imageHidden!: boolean;
  titleHidden!: boolean;
  componentSettings? : ComponentSettingsModel | undefined;
}
