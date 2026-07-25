import {SettingsModel} from "./settings.Model";
import {IconModel} from "./icon.Model";
import {CardTypes} from "../types/card.types.enum";

export class CardModel {
  id!: number;
  indexPosition!: number;
  rowPosition?: number;
  type?: CardTypes | string;
  iconUrl?: string;
  name!: string;
  url!: string;
  iconData?: IconModel;
  settings? : SettingsModel;
}
