import {ComponentSettingsModel} from "./ComponentSettings.Model";
import {IconModel} from "./Icon.Model";

export class ComponentModel {
  id!: number;
  index!: number;
  name!: string;
  url!: string;
  iconData?: IconModel | undefined;
  iconUrl?: string;
  componentSettings? : ComponentSettingsModel | undefined;
}
