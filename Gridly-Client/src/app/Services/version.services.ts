import {Injectable} from "@angular/core";
import {VersionModel} from "../Models/Version.Model";
import {VersionEndpointService} from "./endpoints/version.endpoint.service";
import {VersionEndPointType} from "../Types/endPoint.type.enum";
import {lastValueFrom} from "rxjs";
import {TextStringsUtil} from "../Constants/text.strings.util";
import {MapVersionData} from "../Utils/versionModel.factory";
import {version} from '../../../package.json';

@Injectable({providedIn: 'root'})
export class VersionService {
  private versionModel: VersionModel = new VersionModel();
  private readonly clientVersion: string = version;
  private latestVersion!: VersionModel;
  constructor(private endpointService: VersionEndpointService) {}

  get Version(): VersionModel{
    return this.versionModel;
  }
  set Version(version: VersionModel){
    this.versionModel = version;
  }

  async CallEndPoint(endPointType: VersionEndPointType) : Promise<VersionModel>{
    switch(endPointType){
      case VersionEndPointType.GetCurrentVersion:
        try {
          this.Version = await lastValueFrom(this.endpointService.GetVersion());
          console.log(TextStringsUtil.GetComponentsSucceededEndPointSuccessMessage, this.Version);
        } catch (err) {
          console.error(TextStringsUtil.GetComponentsFailedEndPointMessage, err);
        }
        console.log(this.Version);
        break;
        case VersionEndPointType.GetLatestVersion:
          try {
            this.Version = await lastValueFrom(this.endpointService.GetLatestVersion());
            console.log(TextStringsUtil.GetComponentsSucceededEndPointSuccessMessage, this.Version);
          } catch (err) {
            console.error(TextStringsUtil.GetComponentsFailedEndPointMessage, err);
          }
          break;
        case VersionEndPointType.AddVersion:
          debugger;
          try {
            debugger;
            this.Version = await lastValueFrom(this.endpointService.StoreVersion(this.Version));
            console.log(TextStringsUtil.ComponentAddedEndPointSucceededMessage, this.Version);
          } catch (err) {
            console.error(TextStringsUtil.ComponentAddedFailedEndPointMessage, err);
          }
          break;
    }
    return this.Version;
  }

  public async SetVersion(): Promise<VersionModel>
  {
    this.Version = await this.CallEndPoint(VersionEndPointType.GetCurrentVersion);
    if(this.latestVersion === undefined || this.clientVersion !== this.Version.name)
    {
        this.latestVersion = await this.CallEndPoint(VersionEndPointType.GetLatestVersion);
        if (this.latestVersion !== undefined)
        {
          this.Version = MapVersionData.Override({newRelease: this.clientVersion !== this.latestVersion.name}, this.latestVersion);
          await this.CallEndPoint(VersionEndPointType.AddVersion);
        }
    }
    return this.Version;
  }
}
