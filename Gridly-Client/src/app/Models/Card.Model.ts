import {ComponentSettingsModel} from "./ComponentSettings.Model";
import {IconModel} from "./Icon.Model";

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
  componentSettings? : ComponentSettingsModel |
    {
      width:250,
      height:250,
      imageHidden:false,
      titleHidden:false
    };
}
