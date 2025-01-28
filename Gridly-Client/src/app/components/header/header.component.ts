import {Component, Injectable} from "@angular/core";
import {HandleComponent} from "../modals/handleComponent/handle.component";

@Component({
  selector: 'header-component',
  standalone: true,
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
  imports: [
    HandleComponent
  ]
})

@Injectable({ providedIn: 'root' })
export class HeaderComponent{
  modalTitle = "Add";
  modalButtonTheme ="btn btn-primary";
  modalButtonIcon = "bi bi-plus";
  modelBindId = "addComponentModalLabel";
  modalDropDownId = "addComponentModal";

  constructor(public handleComponent: HandleComponent) {}

}
