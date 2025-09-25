import {ComponentModel} from "../Models/Component.Model";

export function MapComponentData(component?: ComponentModel) : ComponentModel{
  return {
    id: component?.id ?? 0,
    name: component?.name ?? "",
    url: component?.url ?? "",
    iconData: component?.iconData ?? null,
    iconUrl: component?.iconUrl ?? "",
    componentSettings: component?.componentSettings ?? null
  } as ComponentModel;
}
MapComponentData.Override = function(override: Partial<ComponentModel>, component?: ComponentModel): ComponentModel {
  return MapComponentData({
    id: override.id ?? component?.id ?? 0,
    name: override.name ??  component?.name ?? "",
    url: override.url ??  component?.url ?? "",
    iconData: override.iconData ?? component?.iconData ?? null,
    iconUrl: override.iconUrl ?? component?.iconUrl ?? "",
    componentSettings: override.componentSettings ?? component?.componentSettings ?? null
  } as ComponentModel);
};
