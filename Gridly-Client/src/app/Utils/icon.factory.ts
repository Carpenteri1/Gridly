import {IconModel} from "../Models/Icon.Model";

export function MapIconData(iconModel?: IconModel) : IconModel{
  return {
    name: iconModel?.name ?? "",
    type: iconModel?.type ?? "",
    base64Data: iconModel?.base64Data ?? "",
  } as IconModel;
}
MapIconData.Override = function(override: Partial<IconModel>, iconModel?: IconModel): IconModel {
  return MapIconData({
    name: override.name ??  iconModel?.name ?? "",
    type: override?.type ?? iconModel?.type ?? "",
    base64Data: override?.base64Data ?? iconModel?.base64Data ?? "",
  } as IconModel);
};
