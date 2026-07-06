import {SettingsModel} from "./settings.Model";
import {IconModel} from "./icon.Model";

export class CardModel {
  id!: number;
  indexPosition!: number;
  rowPosition?: number;
  iconUrl?: string;
  name!: string;
  url!: string;
  iconData?: IconModel;
  settings? : SettingsModel;
}
