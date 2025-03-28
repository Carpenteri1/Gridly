import {Component, Injectable, OnInit} from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedService } from '../../shared.service';
import {HandleComponent} from "../modals/handleComponent/handle.component";
import {AppComponent} from "../../app.component";

@Component({
  selector: 'basic-component',
  imports: [CommonModule, HandleComponent],
  templateUrl: './basic.component.html',
  standalone: true,
  styleUrls: ['./basic.component.css']
})

@Injectable({ providedIn: 'root' })
export class BasicComponent implements OnInit {
  modalTitle = "Edit";
  modalButtonTheme ="btn btn-modal";
  modalButtonIcon = "bi bi-three-dots";
  modelBindId = "editComponentModalLabel";
  modalDropDownId = "editComponentModal";

  constructor(public sharedService: SharedService, public handleComponent: HandleComponent) {}

  ngOnInit() {
    this.sharedService.LoadComponentList();
  }

  HaveIconSet(name:string | undefined):boolean{
    return name !== undefined && name != null && name !== "";
  }
  HaveImagUrlSet(imageUrl:string | undefined):boolean{
    return imageUrl !== undefined && imageUrl != null && imageUrl !== "";
  }

  IconFilePath(name:string | undefined, fileType:string | undefined): string {
    return "Assets/Icons/" + name + "." + fileType;
  }

  Remove(id: number): void {
    this.sharedService.RemoveComponent(id);
  }
  protected readonly AppComponent = AppComponent;
}
