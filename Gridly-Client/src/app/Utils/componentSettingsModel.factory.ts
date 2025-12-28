import {ComponentSettingsModel} from "../Models/ComponentSettings.Model";

export function MapComponentSettingsData(settings?: ComponentSettingsModel) : ComponentSettingsModel {
  return {
    height: settings?.height ?? 250,
    width: settings?.width ?? 250,
    titleHidden: settings?.titleHidden ?? false,
    imageHidden: settings?.imageHidden ?? false,
  } as ComponentSettingsModel;
}

MapComponentSettingsData.Override = function(override: Partial<ComponentSettingsModel>, settings?: ComponentSettingsModel): ComponentSettingsModel {
  return MapComponentSettingsData({
    height: override?.height ?? settings?.height ?? 250,
    width: override?.width ?? settings?.width ?? 250,
    titleHidden: override?.titleHidden ?? settings?.titleHidden ?? false,
    imageHidden: override?.imageHidden ?? settings?.imageHidden ?? false,
  } as ComponentSettingsModel);
};
