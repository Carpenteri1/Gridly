import {IconModel} from "../Models/Icon.Model";

export function MapIconData(icon?: IconModel) : IconModel {
  return {
    name: icon?.name ?? "",
    type: icon?.type ?? "",
    base64Data: icon?.base64Data ?? "",
    materialIcon: icon?.materialIcon ?? "bi bi-box",
  } as IconModel;
}
MapIconData.Override = function(override: Partial<IconModel>, icon?: IconModel): IconModel {
  return MapIconData({
    name: override?.name ?? icon?.name ?? "",
    type: override?.type ?? icon?.type ?? "",
    base64Data: override?.base64Data ?? icon?.base64Data ?? "",
    materialIcon: override?.materialIcon ?? icon?.materialIcon ?? "bi bi-box",
  } as IconModel);
};
