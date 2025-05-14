import { Component, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { MatFormField, MatLabel } from "@angular/material/input";
import { MatOption } from "@angular/material/core";
import { MatSelect } from "@angular/material/select";
import { MatIcon } from "@angular/material/icon";
import { HandleComponent } from "../Modals/HandleComponents/handle.component"
import { TextStringsUtil } from "../../Utils/text.strings.util";
import { VersionEndpointService } from "../../Services/version.endpoint.service";
import { UrlStringsUtil } from "../../Utils/url.strings.util";

@Component({
  selector: 'header-component',
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
  standalone: true,
  imports: [CommonModule, HandleComponent, MatFormField, MatOption, MatSelect, MatLabel, MatIcon]
})

export class HeaderComponent implements OnInit{
  protected readonly StringUtil = TextStringsUtil;
  constructor(public versionEndpointService: VersionEndpointService) {}

  ngOnInit() {
    this.versionEndpointService.CheckForNewRelease();
    if(this.versionEndpointService.GetVersionName() === ''){
      this.versionEndpointService.CheckForCurrentRelease();
    }
  }

  protected readonly UrlStringsUtil = UrlStringsUtil;
}
