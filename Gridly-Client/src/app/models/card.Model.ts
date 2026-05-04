import {SettingsModel} from "./settings.Model";
import {IconModel} from "./icon.Model";

export class CardModel {
  id!: number;
  indexPosition!: number;
  name!: string;
  url!: string;
  iconData?: IconModel |
    {
      type: "",
      name: "",
      base64Data: "",
      materialIcon: ""
    };
  iconUrl?: string;
  materialIcon?: string;
  settings? : SettingsModel |
    {
      width:250,
      height:250,
      imageHidden:false,
      titleHidden:false
    };
}
