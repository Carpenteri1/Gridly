import {Injectable} from "@angular/core";
import {VersionModel} from "../Models/Version.Model";
import {VersionEndpointService} from "./endpoints/version.endpoint.service";
import {lastValueFrom} from "rxjs";
import {TextStringsUtil} from "../Constants/text.strings.util";
import {MapVersionData} from "../Utils/versionModel.factory";
import {version} from '../../../package.json';

@Injectable({providedIn: 'root'})
export class VersionService {
  private versionModel: VersionModel = new VersionModel();
  private readonly clientVersion: string = version;
  constructor(private endpointService: VersionEndpointService) {}

  get Version(): VersionModel{
    return this.versionModel;
  }
  set Version(version: VersionModel){
    this.versionModel = version;
  }

  async CallEndPoint() : Promise<VersionModel>{
    try {
      this.Version = await lastValueFrom(this.endpointService.GetVersion());
    } catch (err) {
      console.error(TextStringsUtil.GetComponentsFailedEndPointMessage, err);
    }
    return this.Version;
  }

  public async SetVersion(): Promise<VersionModel>
  {
    this.Version = await this.CallEndPoint();

    if(this.clientVersion !== this.Version.name)
      this.Version = MapVersionData.Override({newRelease: this.clientVersion !== this.Version.name}, this.Version);
    return this.Version;
  }
}
