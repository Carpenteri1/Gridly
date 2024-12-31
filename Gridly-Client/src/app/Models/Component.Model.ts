import {IconModel} from "./Icon.Model";

export class ComponentModel {
  id: number;
  name: string;
  url: string;
  iconData: IconModel;
  constructor(id: number, name: string, url: string, iconData: IconModel) {
    this.id = id;
    this.name = name;
    this.url = url;
    this.iconData = iconData;
  }
}
