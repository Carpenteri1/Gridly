import {ComponentModel} from "../Models/Component.Model";

export function MapComponentData(component?: ComponentModel, overrides: Partial<ComponentModel> = {}): ComponentModel {
  return {
    id: overrides.id ?? component?.id ?? 0,
    name: overrides.name ?? component?.name ?? "",
    url: overrides.url ?? component?.url ?? "",
    iconData: overrides.iconData ?? component?.iconData ?? null,
    iconUrl: overrides.iconUrl ?? component?.iconUrl ?? "",
    imageHidden: overrides.imageHidden ?? component?.imageHidden ?? false,
    titleHidden: overrides.titleHidden ?? component?.titleHidden ?? false,
    componentSettings: overrides.componentSettings ?? component?.componentSettings ?? null
  } as ComponentModel;
}
