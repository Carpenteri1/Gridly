import { Component, Injectable, OnInit, ViewEncapsulation } from "@angular/core";
import { HandleComponent } from "../modals/handleComponent/handle.component";
import { SharedService } from '../../Services/shared.service';
import { CommonModule } from "@angular/common";
import { MatFormField, MatLabel } from "@angular/material/input";
import { MatOption } from "@angular/material/core";
import { MatSelect } from "@angular/material/select";

@Component({
  selector: 'header-component',
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
  standalone: true,
  encapsulation: ViewEncapsulation.None,
  imports: [HandleComponent, CommonModule, MatFormField, MatOption, MatSelect, MatLabel]
})

@Injectable({ providedIn: 'root' })
export class HeaderComponent implements OnInit{
  modalTitle = "Add";
  modalButtonTheme ="btn btn-primary";
  modalButtonIcon = "bi bi-plus";
  modelBindId = "addComponentModalLabel";
  modalDropDownId = "addComponentModal";
  constructor(public handleComponent: HandleComponent, public sharedService: SharedService) {}

  ngOnInit() {
    this.sharedService.CheckForNewRelease();
    if(this.sharedService.version.name === ''){
      this.sharedService.CheckForCurrentRelease();
    }
  }
}
