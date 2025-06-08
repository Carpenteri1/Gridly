import {IComponentModel} from "../Models/IComponent.Model";
import {IIconModel} from "../Models/IIcon.Model";

export function CreateComponentData(overrides: Partial<IComponentModel> = {}): IComponentModel {
  return {
    id: overrides.id || 0,
    name: overrides.name || "",
    url: overrides.url || "",
    iconData: overrides.iconData || {fileType: "", name: "", base64Data: ""},
    imageUrl: overrides.imageUrl || "",
    imageHidden: overrides.imageHidden || false,
    titleHidden: overrides.titleHidden || false,
    editMode: overrides.editMode || false,
    componentSettings: overrides.componentSettings || {width: 0, height: 0}
  };
}

export function CreateIconData(overrides: Partial<IIconModel> = {}): IIconModel {
  return {
    fileType: overrides.fileType || "",
    name: overrides.name || "",
    base64Data: overrides.base64Data || "",
  };
}
