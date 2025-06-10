import {ComponentModel} from "../Models/Component.Model";
import {IconModel} from "../Models/Icon.Model";

export function SetComponentData(component?: ComponentModel, overrides: Partial<ComponentModel> = {}): ComponentModel {
  return {
    id: overrides.id ?? component?.id ?? 0,
    name: overrides.name ?? component?.name ?? "",
    url: overrides.url ?? component?.url ?? "",
    iconData: SetIconData(overrides.iconData ?? component?.iconData ?? undefined),
    imageUrl: overrides.imageUrl ?? component?.imageUrl ?? "",
    imageHidden: overrides.imageHidden ?? component?.imageHidden ?? false,
    titleHidden: overrides.titleHidden ?? component?.titleHidden ?? false,
    editMode: overrides.editMode ?? component?.editMode ?? false,
    componentSettings: overrides.componentSettings ?? component?.componentSettings ?? {width: 0, height: 0}
  } as ComponentModel;
}

export function SetIconData(iconData?: IconModel ,overrides: Partial<IconModel> = {}): IconModel {
  return {
    fileType: overrides.fileType ?? iconData?.fileType ?? "",
    name: overrides.name ?? iconData?.name ?? "",
    base64Data: overrides.base64Data ?? iconData?.base64Data ?? "",
  };
}
