import { Component } from "@angular/core";
import {AddComponent} from "../Modals/AddComponent/add.component";

@Component({
  selector: 'header-component',
  standalone: true,
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
  imports: [
    AddComponent
  ]
})

export class HeaderComponent{

  protected readonly AddComponent = AddComponent;
}
