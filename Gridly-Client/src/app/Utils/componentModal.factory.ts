import {ComponentModel} from "../Models/Component.Model";

export function MapComponentData(component?: ComponentModel, override: Partial<ComponentModel> = {}) : ComponentModel{
  return {
    id: component?.id ?? override.id ?? 0,
    name: component?.name ?? override.name ?? "",
    url: component?.url ?? override.url ?? "",
    iconData: component?.iconData ?? override.iconData ?? null,
    iconUrl: component?.iconUrl ?? override.iconUrl ?? "",
    imageHidden: component?.imageHidden ?? override.imageHidden ?? false,
    titleHidden: component?.titleHidden ?? override.titleHidden ?? false,
    componentSettings: component?.componentSettings ?? override.componentSettings ?? null
  } as ComponentModel;
}
MapComponentData.Override = function(component: ComponentModel, overrides: Partial<ComponentModel>): ComponentModel {
  return MapComponentData(component, overrides);
};
