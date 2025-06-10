import {ComponentSettingsModel} from "./ComponentSettings.Model";
import {IconModel} from "./Icon.Model";

export class ComponentModel {
  id!: number;
  name!: string;
  url!: string;
  iconData!: IconModel | undefined;
  imageUrl!: string;
  imageHidden!: boolean;
  titleHidden!: boolean;
  editMode!: boolean;
  resizeMode!: boolean;
  dragMode!: boolean;
  componentSettings! : ComponentSettingsModel;
  constructor(init?: Partial<ComponentModel>) {
    Object.assign(this, init);
  }
}
