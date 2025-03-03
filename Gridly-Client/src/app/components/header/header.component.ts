import {Component, Injectable, OnInit} from "@angular/core";
import {HandleComponent} from "../modals/handleComponent/handle.component";
import {SharedService} from "../../shared.service";
import {CommonModule} from "@angular/common";

@Component({
  selector: 'header-component',
  standalone: true,
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
  imports: [
    HandleComponent,
    CommonModule
  ]
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
  }
}
