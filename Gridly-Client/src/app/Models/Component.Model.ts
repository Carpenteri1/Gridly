import {IconModel} from "./Icon.Model";

export class ComponentModel {
  id: number;
  name: string;
  url: string;
  iconData?: IconModel;
  imageUrl?: string;
  constructor(id: number, name: string, url: string, iconData?: IconModel, imageUrl?: string) {
    this.id = id;
    this.name = name;
    this.url = url;
    this.iconData = iconData;
    this.imageUrl = imageUrl;
  }
}
