export class ComponentModel {
  id: number;
  name: string;
  url: string;
  iconData: string;
  constructor(id: number, name: string, url: string, iconData: string) {
    this.id = id;
    this.name = name;
    this.url = url;
    this.iconData = iconData;
  }
}
