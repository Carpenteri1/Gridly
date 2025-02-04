import {IconModel} from "./Icon.Model";

export class ComponentModel {
  id: number;
  name: string;
  url: string;
  iconData?: IconModel;
  imageUrl?: string;
  imageHidden?: boolean;
  titleHidden?: boolean;
  constructor(id: number, name: string, url: string, iconData?: IconModel, imageUrl?: string | undefined, imageHidden?: boolean, titleHidden?: boolean) {
    this.id = id;
    this.name = name;
    this.url = url;
    this.iconData = iconData;
    this.imageUrl = imageUrl;
    this.imageHidden = imageHidden;
    this.titleHidden = titleHidden;
  }
}
