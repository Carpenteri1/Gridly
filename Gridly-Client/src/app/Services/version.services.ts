import {Injectable} from "@angular/core";
import {VersionModel} from "../Models/Version.Model";
import {VersionEndpointService} from "./endpoints/version.endpoint.service";
import {VersionEndPointType} from "../Types/endPoint.type.enum";
import {lastValueFrom} from "rxjs";
import {TextStringsUtil} from "../Constants/text.strings.util";

@Injectable({providedIn: 'root'})
export class VersionService {
  private version!: VersionModel;
  constructor(private endpointService: VersionEndpointService) {}

  get Version(): VersionModel{
    return this.version;
  }
  set Version(version: VersionModel){
    this.version = version;
  }

  async CallEndPoint(endPointType: VersionEndPointType){
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
          console.log(this.Version);
          break;
        case VersionEndPointType.AddVersion:
          try {
            this.Version = await lastValueFrom(this.endpointService.StoreVersion(this.Version));
            console.log(TextStringsUtil.ComponentAddedEndPointSucceededMessage, this.Version);
          } catch (err) {
            console.error(TextStringsUtil.ComponentAddedFailedEndPointMessage, err);
          }
          break;
    }
  }
}
