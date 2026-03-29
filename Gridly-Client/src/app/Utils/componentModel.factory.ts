import {ComponentModel} from "../Models/Component.Model";
import {MapComponentSettingsData} from "./componentSettingsModel.factory";
import { MapIconData } from "./iconModel.factory";

export function MapComponentData(component?: ComponentModel) : ComponentModel{
  return {
    id: component?.id ?? 0,
    indexPosition: component?.indexPosition ?? 0,
    name: component?.name ?? "",
    url: component?.url ?? "",
    iconData: component?.iconData ?? MapIconData(),
    iconUrl: component?.iconUrl ?? "",
    componentSettings: component?.componentSettings ?? MapComponentSettingsData()
  } as ComponentModel;
}
MapComponentData.Override = function(override: Partial<ComponentModel>, component?: ComponentModel): ComponentModel {
  return MapComponentData({
    id: override.id ?? component?.id ?? 0,
    indexPosition: override.indexPosition ?? component?.indexPosition ?? 0,
    name: override.name ??  component?.name ?? "",
    url: override.url ??  component?.url ?? "",
    iconData: override.iconData ?? component?.iconData ?? MapIconData(),
    iconUrl: override.iconUrl ?? component?.iconUrl ?? "",
    materialIcon: override.materialIcon ?? component?.materialIcon ?? "",
    componentSettings: override.componentSettings ?? component?.componentSettings ?? MapComponentSettingsData()
  } as ComponentModel);
};  
