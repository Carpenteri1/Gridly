import {VersionModel} from "../Models/Version.Model";

export function MapVersionData(version?: VersionModel) : VersionModel {
  return {
    name: version?.name ?? "",
    newRelease: version?.newRelease ?? false
  } as VersionModel;
}
MapVersionData.Override = function(override: Partial<VersionModel>, component?: VersionModel): VersionModel {
  return MapVersionData({
    name: override.name ??  component?.name ?? "0.0.0-alpha",
    newRelease: override?.newRelease ?? false
  } as VersionModel);
};
