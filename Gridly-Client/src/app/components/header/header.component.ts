import { Component, OnInit } from "@angular/core";
import { SharedService } from '../../Services/shared.service';
import { CommonModule } from "@angular/common";
import { MatFormField, MatLabel } from "@angular/material/input";
import { MatOption } from "@angular/material/core";
import { MatSelect } from "@angular/material/select";
import { MatIcon } from "@angular/material/icon";
import { HandleComponent } from "../modals/handleComponent/handle.component";
import { StringUtil } from "../../Utils/string.util";

@Component({
  selector: 'header-component',
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
  standalone: true,
  imports: [CommonModule, HandleComponent, MatFormField, MatOption, MatSelect, MatLabel, MatIcon]
})

export class HeaderComponent implements OnInit{
  protected readonly StringUtil = StringUtil;
  constructor(public sharedService: SharedService) {}

  ngOnInit() {
    this.sharedService.CheckForNewRelease();
    if(this.sharedService.version.name === ''){
      this.sharedService.CheckForCurrentRelease();
    }
  }

  AddComponent() {
    this.sharedService.AddComponent(this.sharedService.flexItems[0]);
  }
}
